using KafkaTest.Clients;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using RestCore.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestCore
{
    public class MyClient
    {

        //public static List<BatchJobQueuestruct> batchJobQueues = new List<BatchJobQueuestruct>();


        const int ReconnectPeriod = 10;
        Session session;
        SessionReconnectHandler reconnectHandler;
        string endpointURL;
        int clientRunTime = Timeout.Infinite;
        static bool autoAccept = false;
        static ExitCode exitCode;

        static IKafkaClient kafkaClient = new KafkaClient(null);

        public MyClient(string _endpointURL, bool _autoAccept, int _stopTimeout)
        {
            endpointURL = _endpointURL;
            autoAccept = _autoAccept;
            clientRunTime = _stopTimeout <= 0 ? Timeout.Infinite : _stopTimeout * 1000;

        }



        public void Run()
        {
            try
            {
                ConsoleClient().Wait();
            }
            catch (Exception ex)
            {
                Utils.Trace("ServiceResultException:" + ex.Message);
                Console.WriteLine("Exception: {0}", ex.Message);
                return;
            }

            ManualResetEvent quitEvent = new ManualResetEvent(false);
            try
            {
                Console.CancelKeyPress += (sender, eArgs) =>
                {
                    quitEvent.Set();
                    eArgs.Cancel = true;
                };
            }
            catch
            {
            }

            // wait for timeout or Ctrl-C
            quitEvent.WaitOne(clientRunTime);

            // return error conditions
            if (session.KeepAliveStopped)
            {
                exitCode = ExitCode.ErrorNoKeepAlive;
                return;
            }

            exitCode = ExitCode.Ok;
        }

        public static ExitCode ExitCode { get => exitCode; }

        private async Task ConsoleClient()
        {
            Console.WriteLine("1 - Create an Application Configuration.");
            var config = new ApplicationConfiguration()
            {
                ApplicationName = "ClientTest",
                ApplicationUri = Utils.Format(@"urn:{0}:ClientTest", System.Net.Dns.GetHostName()),
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = Utils.Format(@"CN={0}, DC={1}", "ClientTest", System.Net.Dns.GetHostName()) },
                    TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                    TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                    RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                    AutoAcceptUntrustedCertificates = true,
                    AddAppCertToTrustedStore = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas
                {
                    OperationTimeout = 600000,
                    MaxStringLength = Int32.MaxValue,
                    MaxByteStringLength = Int32.MaxValue,
                    MaxArrayLength = Int32.MaxValue,
                    MaxMessageSize = Int32.MaxValue,
                    MaxBufferSize = Int16.MaxValue
                },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
                TraceConfiguration = new TraceConfiguration(),
                ServerConfiguration = new ServerConfiguration
                {
                    MaxSubscriptionCount = int.MaxValue
                }
            };
            config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
            if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
            }

            var application = new ApplicationInstance
            {
                ApplicationName = "ClientTest",
                ApplicationType = ApplicationType.Client,
                ApplicationConfiguration = config
            };
            application.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();




            Console.WriteLine("2 - Discover endpoints of {0}.", endpointURL);
            exitCode = ExitCode.ErrorDiscoverEndpoints;
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointURL, useSecurity: true, operationTimeout: 15000);
            Console.WriteLine("    Selected endpoint uses: {0}",
                selectedEndpoint.SecurityPolicyUri.Substring(selectedEndpoint.SecurityPolicyUri.LastIndexOf('#') + 1));


            Console.WriteLine("3 - Create a session with OPC UA server.");
            exitCode = ExitCode.ErrorCreateSession;
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            session = await Session.Create(config, endpoint, false, "OPC UA Console Client", 60000, new UserIdentity(new AnonymousIdentityToken()), null);

            // register keep alive handler
            session.KeepAlive += Client_KeepAlive;

            Console.WriteLine("4 - Browse the OPC UA server namespace.");
            exitCode = ExitCode.ErrorBrowseNamespace;
            ReferenceDescriptionCollection references;
            Byte[] continuationPoint;

            references = session.FetchReferences(ObjectIds.ObjectsFolder);

            session.Browse(
                null,
                null,
                ObjectIds.ObjectsFolder,
                0u,
                BrowseDirection.Forward,
                ReferenceTypeIds.HierarchicalReferences,
                true,
                (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                out continuationPoint,
                out references);

            Console.WriteLine(" DisplayName, BrowseName, NodeClass");
            foreach (var rd in references)
            {
                Console.WriteLine(" {0}, {1}, {2}", rd.DisplayName, rd.BrowseName, rd.NodeClass);
                ReferenceDescriptionCollection nextRefs;
                byte[] nextCp;
                session.Browse(
                    null,
                    null,
                    ExpandedNodeId.ToNodeId(rd.NodeId, session.NamespaceUris),
                    0u,
                    BrowseDirection.Forward,
                    ReferenceTypeIds.HierarchicalReferences,
                    true,
                    (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                    out nextCp,
                    out nextRefs);

                foreach (var nextRd in nextRefs)
                {
                    Console.WriteLine("   + {0}, {1}, {2}", nextRd.DisplayName, nextRd.BrowseName, nextRd.NodeClass);
                }
            }

            //init the batches before sub so the sub is much faster
            Init_batches();

            

            Console.WriteLine("5 - Create a subscription with publishing interval of 1 second. ");
            exitCode = ExitCode.ErrorCreateSubscription;
            var subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 100 };

            Console.WriteLine("6 - Add a list of items (server current time and status) to the subscription.");
            exitCode = ExitCode.ErrorMonitoredItem;


            #region subs
            var list = new List<MonitoredItem> {
                //new MonitoredItem(subscription.DefaultItem)
                //{
                //    DisplayName = "ServerStatusCurrentTime", StartNodeId = "i="+Variables.Server_ServerStatus_CurrentTime.ToString()
                //    DisplayName = "InstandShutdown",  StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC_POU.started"
                //},
                new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "batchJobQueue.AmountBatchJobs",
                    StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.AmountBatchJobs",
                },
                new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "batchJobQueue.NextFreePos",
                    StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.NextFreePos",
                },
                new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "batchJobQueue.Full",
                    StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.Full",
                }
            };

            //BatchJobs

            

            for (int i = 1; i <= Program.maxBatchJobs; i++)
            {
                list.Add(new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "batchJobQueue.batchJobs[" + i + "].BatchSize",
                    StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].BatchSize",
                });

                //maxWorkpieces = Int32.Parse(Read_node("batchJobs[" + i + "].batchSize"));

                for (int j = 1; j <= Program.maxWorkpieces; j++)
                {
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Color",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Color",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].LinePos",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].LinePos",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].SortingLinePos",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].SortingLinePos",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].State",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].State",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].StartPos",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].StartPos",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].EndPos",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].EndPos",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.FurEnabled",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.FurEnabled",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.SawEnabled",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.SawEnabled",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.ColorCheckEnabled",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.ColorCheckEnabled",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.EjectEnabled",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.EjectEnabled",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.FurDuration",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.FurDuration",
                    });
                    list.Add(new MonitoredItem(subscription.DefaultItem)
                    {
                        DisplayName = "batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.SawDuration",
                        StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].workpieces[" + j + "].Format.SawDuration",
                    });

                }
                list.Add(new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "batchJobQueue.batchJobs[" + i + "].NextFreeWPos",
                    StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].NextFreeWPos",
                });
                list.Add(new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "batchJobQueue.batchJobs[" + i + "].AmountFinishedW",
                    StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].AmountFinishedW",
                });
                list.Add(new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "batchJobQueue.batchJobs[" + i + "].NextNotProcessedIdx",
                    StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].NextNotProcessedIdx",
                });
                list.Add(new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "batchJobQueue.batchJobs[" + i + "].Mode",
                    StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].Mode",
                });
                list.Add(new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "batchJobQueue.batchJobs[" + i + "].state",
                    StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue.batchJobs[" + i + "].state",
                });
            }

            //SortingLine

            //list.Add(new MonitoredItem(subscription.DefaultItem)
            //{
            //    DisplayName = "SortingLine.colorSensor",
            //    StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.colorSensor",
            //});
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.compressor",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.compressor",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.iFeeler",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.iFeeler",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.lbAfterColorSensor",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.lbAfterColorSensor",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.lbBlue",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.lbBlue",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.lbRed",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.lbRed",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.lbWhite",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.lbWhite",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.mConveyorBelt",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.mConveyorBelt",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.valveFirstEject",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.valveFirstEject",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.valveSecEject",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.valveSecEject",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.valveThirdEject",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.valveThirdEject",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.state",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.state",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.bIdx",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.bIdx",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.any_motor_running",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.any_motor_running",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "SortingLine.wIdx",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.SL.wIdx",
            });


            //furnance
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.sRaPosSucker",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.sRaPosSucker",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.sRaPosConveyorBelt",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.sRaPosConveyorBelt",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.lbConveyorBelt",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.lbConveyorBelt",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.sRaPosSaw",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.sRaPosSaw",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.sSuckerPosRa",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.sSuckerPosRa",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.sFurnanceSliderInside",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.sFurnanceSliderInside",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.sFurnanceSliderOutside",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.sFurnanceSliderOutside",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.sSuckerPosFF",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.sSuckerPosFF",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.mRaClockwise",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.mRaClockwise",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.mRaCClockwise",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.mRaCClockwise",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.mConveyorBeltForward",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.mConveyorBeltForward",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.mSaw",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.mSaw",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.mFurnanceSliderIn",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.mFurnanceSliderIn",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.mFurnanceSliderOut",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.mFurnanceSliderOut",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.mSuckertoFurnance",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.mSuckertoFurnance",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.mSuckertoRa",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.mSuckertoRa",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.compressor",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.compressor",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.valveVacuum",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.valveVacuum",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.valveLowering",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.valveLowering",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.valveFurnanceDoor",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.valveFurnanceDoor",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.valveSlider",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.valveSlider",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.any_motor_running",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.any_motor_running",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.state",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.state",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.bIdxBurner",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.bIdxBurner",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.bIdxSuccer",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.bIdxSuccer",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.bIdxSaw",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.bIdxSaw",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.wIdxBurner",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.wIdxBurner",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.wIdxSuccer",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.wIdxSuccer",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Furnance.wIdxSaw",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.FUR.wIdxSaw",
            });

            //highrack

            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.sHorizontal",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.sHorizontal",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.lbIn",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.lbIn",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.lbOut",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.lbOut",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.sVertical",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.sVertical",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.sCFront",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.sCFront",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.sCBack",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.sCBack",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.encHorizontal",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.encHorizontal",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.encVertical",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.encVertical",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.mCbForward",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.mCbForward",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.mCbBackward",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.mCbBackward",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.mHorizontalLeft",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.mHorizontalLeft",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.mHorizontalRight",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.mHorizontalRight",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.mVerticalDown",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.mVerticalDown",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.mVerticalUp",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.mVerticalUp",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.mCForward",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.mCForward",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.mCBackward",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.mCBackward",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.state",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.state",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.any_motor_running",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.any_motor_running",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.cX",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.cX",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.cY",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.cY",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.bIdx",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.bIdx",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "HighRack.wIdx",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.HR.wIdx",
            });

            //taskconfigurator

            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "TaskConfigurator.started",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC_POU.started",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "TaskConfigurator.fin",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC_POU.fin",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "TaskConfigurator.batch_job_available",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC_POU.batch_job_available",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "TaskConfigurator.mode_active",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.mode_active",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "TaskConfigurator.is_plc_running",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.is_plc_running",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "TaskConfigurator.instant_shutdown",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.instant_shutdown",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "TaskConfigurator.currBatchJobIdx",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.currBatchJobIdx",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "TaskConfigurator.state",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.state",
            });

            //vacuumgripper

            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.sVertical",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.sVertical",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.sHorizontal",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.sHorizontal",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.sRotation",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.sRotation",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.encVertical",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.encVertical",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.encHorizontal",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.encHorizontal",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.encRotation",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.encRotation",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.mVerticalUp",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.mVerticalUp",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.mVerticalDown",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.mVerticalDown",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.mHorizontalBackward",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.mHorizontalBackward",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.mHorizontalForward",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.mHorizontalForward",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.mRotationClockwise",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.mRotationClockwise",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.mRotationCClockwise",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.mRotationCClockwise",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.compressor",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.compressor",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.valve",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.valve",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.state",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.state",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.any_motor_running",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.any_motor_running",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.cX",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.cX",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.cY",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.cY",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.cZ",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.cZ",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.bIdx",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.bIdx",
            });
            list.Add(new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "VacuumGripper.wIdx",
                StartNodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.VG.wIdx",
            });


            #endregion


            list.ForEach(i => i.Notification += OnNotification);
            subscription.AddItems(list);


            Console.WriteLine("7 - Add the subscription to the session. ");
            exitCode = ExitCode.ErrorAddSubscription;
            session.AddSubscription(subscription);
            subscription.Create();

            Console.WriteLine("8 - Running...Press Ctrl-C to exit...");


            exitCode = ExitCode.ErrorRunning;
        }

        private static void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                // show
                int anz = item.DisplayName.Length;
                string leer = new string(' ', 70-anz);
                //Console.WriteLine("{0}: \t{1}, \t{2}, \t{3}", item.DisplayName, value.Value, value.StatusCode, value.SourceTimestamp);
                Console.WriteLine("{0}:{1}{2}", item.DisplayName,leer ,value.Value);
                update(item.DisplayName, value.Value);

                // to kafka
                kafkaClient.Produce(item.DisplayName, item.DisplayName + ":" + value.Value);

            }
        }
        /// <summary>
        /// Update the Batches and Legacy
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="value"></param>
        public static void update(string nodes, object value)
        {
            string[] node = nodes.Split('.');

            switch (node[0])
            {
                case "batchJobQueue":
                    switch (node[1])
                    {
                        case "AmountBatchJobs":
                            Program.batchJobQueue.AmountBatchJobs = Convert.ToInt32(value);
                            break;
                        case "NextFreePos":
                            Program.batchJobQueue.NextFreePos = Convert.ToInt32(value);
                            break;
                        case "Full":
                            Program.batchJobQueue.Full = Convert.ToBoolean(value);
                            break;
                        default:
                            int batchnumber = Convert.ToInt16(node[1].Substring(10, 1));
                            switch (node[2])
                            {
                                case "BatchSize":
                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].BatchSize = Convert.ToInt16(value);
                                    break;
                                case "NextFreeWPos":
                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].NextFreeWPos = Convert.ToInt16(value);
                                    break;
                                case "AmountFinishedW":
                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].AmountFinishedW = Convert.ToInt16(value);
                                    break;
                                case "NextNotProcessedIdx":
                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].NextNotProcessedIdx = Convert.ToInt16(value);
                                    break;
                                case "Mode":
                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].Mode = (ExecutionMode)Convert.ToInt16(value);
                                    break;
                                case "state":
                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].state = (BatchJobState)Convert.ToInt16(value);
                                    break;

                                default:
                                    int workpiecenumber = Convert.ToInt16(node[2].Substring(11, 1));
                                    switch (node[3])
                                    {
                                        case "Color":
                                            Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].Color = value.ToString();
                                            break;
                                        case "LinePos":
                                            Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].LinePos = Convert.ToInt16(value);
                                            break;
                                        case "SortingLinePos":
                                            Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].SortingLinePos = Convert.ToInt16(value);
                                            break;
                                        case "StartPos":
                                            Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].StartPos = (RackPos)Convert.ToInt16(value);
                                            break;
                                        case "State":
                                            Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].State = (W_State)Convert.ToInt16(value);
                                            break;
                                        case "EndPos":
                                            Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].EndPos = (RackPos)Convert.ToInt16(value);
                                            break;
                                        default:
                                            //Console.WriteLine(node[4]);
                                            switch (node[4])
                                            {
                                                case "FurEnabled":
                                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].Formate[0].FurEnabled = Convert.ToBoolean(value);
                                                    break;
                                                case "SawEnabled":
                                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].Formate[0].SawEnabled = Convert.ToBoolean(value);
                                                    break;
                                                case "ColorCheckEnabled":
                                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].Formate[0].ColorCheckEnabled = Convert.ToBoolean(value);
                                                    break;
                                                case "EjectEnabled":
                                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].Formate[0].EjectEnabled = Convert.ToBoolean(value);
                                                    break;
                                                case "FurDuration":
                                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].Formate[0].FurDuration = Convert.ToInt32(value);
                                                    break;
                                                case "SawDuration":
                                                    Program.batchJobQueue.BatchJobs[batchnumber - 1].Workpieces[workpiecenumber - 1].Formate[0].SawDuration = Convert.ToInt32(value);
                                                    break;
                                                default:
                                                    Console.ForegroundColor = ConsoleColor.Red;
                                                    Console.WriteLine("Fehler!");
                                                    Console.ResetColor();
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case "Furnance":
                    switch (node[1])
                    {
                        case "sRaPosSucker":
                            Program.furnance.sRaPosSucker = Convert.ToBoolean(value);
                            break;
                        case "sRaPosConveyorBelt":
                            Program.furnance.sRaPosConveyorBelt = Convert.ToBoolean(value);
                            break;
                        case "lbConveyorBelt":
                            Program.furnance.lbConveyorBelt = Convert.ToBoolean(value);
                            break;
                        case "sRaPosSaw":
                            Program.furnance.sRaPosSaw = Convert.ToBoolean(value);
                            break;
                        case "sSuckerPosRa":
                            Program.furnance.sSuckerPosRa = Convert.ToBoolean(value);
                            break;
                        case "sFurnanceSliderInside":
                            Program.furnance.sFurnanceSliderInside = Convert.ToBoolean(value);
                            break;
                        case "sFurnanceSliderOutside":
                            Program.furnance.sFurnanceSliderOutside = Convert.ToBoolean(value);
                            break;
                        case "sSuckerPosFF":
                            Program.furnance.sSuckerPosFF = Convert.ToBoolean(value);
                            break;
                        case "lbFurnance":
                            Program.furnance.lbFurnance = Convert.ToBoolean(value);
                            break;
                        case "mRaClockwise":
                            Program.furnance.mRaClockwise = Convert.ToBoolean(value);
                            break;
                        case "mRaCClockwise":
                            Program.furnance.mRaCClockwise = Convert.ToBoolean(value);
                            break;
                        case "mConveyorBeltForward":
                            Program.furnance.mConveyorBeltForward = Convert.ToBoolean(value);
                            break;
                        case "mSaw":
                            Program.furnance.mSaw = Convert.ToBoolean(value);
                            break;
                        case "mFurnanceSliderIn":
                            Program.furnance.mFurnanceSliderIn = Convert.ToBoolean(value);
                            break;
                        case "mFurnanceSliderOut":
                            Program.furnance.mFurnanceSliderOut = Convert.ToBoolean(value);
                            break;
                        case "mSuckertoFurnance":
                            Program.furnance.mSuckertoFurnance = Convert.ToBoolean(value);
                            break;
                        case "mSuckertoRa":
                            Program.furnance.mSuckertoRa = Convert.ToBoolean(value);
                            break;
                        case "compressor":
                            Program.furnance.compressor = Convert.ToBoolean(value);
                            break;
                        case "valveVacuum":
                            Program.furnance.valveVacuum = Convert.ToBoolean(value);
                            break;
                        case "valveLowering":
                            Program.furnance.valveLowering = Convert.ToBoolean(value);
                            break;
                        case "valveFurnanceDoor":
                            Program.furnance.valveFurnanceDoor = Convert.ToBoolean(value);
                            break;
                        case "valveSlider":
                            Program.furnance.valveSlider = Convert.ToBoolean(value);
                            break;
                        case "any_motor_running":
                            Program.furnance.any_motor_running = Convert.ToBoolean(value);
                            break;
                        case "state":
                            Program.furnance.state = (FUR_State)Convert.ToInt16(value);
                            break;
                        case "bIdxBurner":
                            Program.furnance.bIdxBurner = Convert.ToInt16(value);
                            break;
                        case "bIdxSuccer":
                            Program.furnance.bIdxSuccer = Convert.ToInt16(value);
                            break;
                        case "bIdxSaw":
                            Program.furnance.bIdxSaw = Convert.ToInt16(value);
                            break;
                        case "wIdxBurner":
                            Program.furnance.wIdxBurner = Convert.ToInt16(value);
                            break;
                        case "wIdxSuccer":
                            Program.furnance.wIdxSuccer = Convert.ToInt16(value);
                            break;
                        case "wIdxSaw":
                            Program.furnance.wIdxSaw = Convert.ToInt16(value);
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Fehler!");
                            Console.ResetColor();
                            break;

                    }
                    break;
                case "HighRack":
                    switch (node[1])
                    {
                        case "sHorizontal":
                            Program.highRack.sHorizontal = Convert.ToBoolean(value);
                            break;
                        case "lbIn":
                            Program.highRack.lbIn = Convert.ToBoolean(value);
                            break;
                        case "lbOut":
                            Program.highRack.lbOut = Convert.ToBoolean(value);
                            break;
                        case "sVertical":
                            Program.highRack.sVertical = Convert.ToBoolean(value);
                            break;
                        case "sCFront":
                            Program.highRack.sCFront = Convert.ToBoolean(value);
                            break;
                        case "sCBack":
                            Program.highRack.sCBack = Convert.ToBoolean(value);
                            break;
                        case "encHorizontal":
                            Program.highRack.encHorizontal = Convert.ToBoolean(value);
                            break;
                        case "encVertical":
                            Program.highRack.encVertical = Convert.ToBoolean(value);
                            break;
                        case "mCbForward":
                            Program.highRack.mCbForward = Convert.ToBoolean(value);
                            break;
                        case "mCbBackward":
                            Program.highRack.mCbBackward = Convert.ToBoolean(value);
                            break;
                        case "mHorizontalLeft":
                            Program.highRack.mHorizontalLeft = Convert.ToBoolean(value);
                            break;
                        case "mHorizontalRight":
                            Program.highRack.mHorizontalRight = Convert.ToBoolean(value);
                            break;
                        case "mVerticalDown":
                            Program.highRack.mVerticalDown = Convert.ToBoolean(value);
                            break;
                        case "mVerticalUp":
                            Program.highRack.mVerticalUp = Convert.ToBoolean(value);
                            break;
                        case "mCForward":
                            Program.highRack.mCForward = Convert.ToBoolean(value);
                            break;
                        case "mCBackward":
                            Program.highRack.mCBackward = Convert.ToBoolean(value);
                            break;
                        case "state":
                            Program.highRack.state = (HR_State)Convert.ToInt16(value);
                            break;
                        case "any_motor_running":
                            Program.highRack.any_motor_running = Convert.ToBoolean(value);
                            break;
                        case "cX":
                            Program.highRack.cX = Convert.ToInt16(value);
                            break;
                        case "cY":
                            Program.highRack.cY = Convert.ToInt16(value);
                            break;
                        case "bIdx":
                            Program.highRack.bIdx = Convert.ToInt16(value);
                            break;
                        case "wIdx":
                            Program.highRack.wIdx = Convert.ToInt16(value);
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Fehler!");
                            Console.ResetColor();
                            break;
                    }
                    break;
                case "SortingLine":
                    switch (node[1])
                    {
                        case "colorSensor":
                            Program.sortingLine.colorSensor = Convert.ToInt32(value);
                            break;
                        case "compressor":
                            Program.sortingLine.compressor = Convert.ToBoolean(value);
                            break;
                        case "iFeeler":
                            Program.sortingLine.iFeeler = Convert.ToBoolean(value);
                            break;
                        case "lbAfterColorSensor":
                            Program.sortingLine.lbAfterColorSensor = Convert.ToBoolean(value);
                            break;
                        case "lbEntry":
                            Program.sortingLine.lbEntry = Convert.ToBoolean(value);
                            break;
                        case "lbBlue":
                            Program.sortingLine.lbBlue = Convert.ToBoolean(value);
                            break;
                        case "lbRed":
                            Program.sortingLine.lbRed = Convert.ToBoolean(value);
                            break;
                        case "lbWhite":
                            Program.sortingLine.lbWhite = Convert.ToBoolean(value);
                            break;
                        case "mConveyorBelt":
                            Program.sortingLine.mConveyorBelt = Convert.ToBoolean(value);
                            break;
                        case "valveFirstEject":
                            Program.sortingLine.valveFirstEject = Convert.ToBoolean(value);
                            break;
                        case "valveSecEject":
                            Program.sortingLine.valveSecEject = Convert.ToBoolean(value);
                            break;
                        case "valveThirdEject":
                            Program.sortingLine.valveThirdEject = Convert.ToBoolean(value);
                            break;
                        case "any_motor_running":
                            Program.sortingLine.any_motor_running = Convert.ToBoolean(value);
                            break;
                        case "state":
                            Program.sortingLine.state = (SL_State)Convert.ToInt16(value);
                            break;
                        case "bIdx":
                            Program.sortingLine.bIdx = Convert.ToInt16(value);
                            break;
                        case "wIdx":
                            Program.sortingLine.wIdx = Convert.ToInt16(value);
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Fehler!");
                            Console.ResetColor();
                            break;

                    }
                    break;
                case "TaskConfigurator":
                    switch (node[1])
                    {
                        case "started":
                            Program.taskConfigurator.started = Convert.ToBoolean(value);
                            break;
                        case "fin":
                            Program.taskConfigurator.fin = Convert.ToBoolean(value);
                            break;
                        case "batch_job_available":
                            Program.taskConfigurator.batch_job_available = Convert.ToBoolean(value);
                            break;
                        case "mode_active":
                            Program.taskConfigurator.mode_active = (ExecutionMode)Convert.ToInt16(value);
                            break;
                        case "is_plc_running":
                            Program.taskConfigurator.is_plc_running = Convert.ToBoolean(value);
                            break;
                        case "instant_shutdown":
                            Program.taskConfigurator.instant_shutdown = Convert.ToBoolean(value);
                            break;
                        case "currBatchJobIdx":
                            Program.taskConfigurator.currBatchJobIdx = Convert.ToInt16(value);
                            break;
                        case "state":
                            Program.taskConfigurator.state = (TC_State)Convert.ToInt16(value);
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Fehler!");
                            Console.ResetColor();
                            break;
                    }
                    break;
                case "VacuumGripper":
                    switch (node[1])
                    {
                        case "sVertical":
                            Program.vacuumGripper.sVertical = Convert.ToBoolean(value);
                            break;
                        case "sHorizontal":
                            Program.vacuumGripper.sHorizontal = Convert.ToBoolean(value);
                            break;
                        case "sRotation":
                            Program.vacuumGripper.sRotation = Convert.ToBoolean(value);
                            break;
                        case "encVertical":
                            Program.vacuumGripper.encVertical = Convert.ToBoolean(value);
                            break;
                        case "encHorizontal":
                            Program.vacuumGripper.encHorizontal = Convert.ToBoolean(value);
                            break;
                        case "encRotation":
                            Program.vacuumGripper.encRotation = Convert.ToBoolean(value);
                            break;
                        case "mVerticalUp":
                            Program.vacuumGripper.mVerticalUp = Convert.ToBoolean(value);
                            break;
                        case "mVerticalDown":
                            Program.vacuumGripper.mVerticalDown = Convert.ToBoolean(value);
                            break;
                        case "mHorizontalBackward":
                            Program.vacuumGripper.mHorizontalBackward = Convert.ToBoolean(value);
                            break;
                        case "mHorizontalForward":
                            Program.vacuumGripper.mHorizontalForward = Convert.ToBoolean(value);
                            break;
                        case "mRotationClockwise":
                            Program.vacuumGripper.mRotationClockwise = Convert.ToBoolean(value);
                            break;
                        case "mRotationCClockwise":
                            Program.vacuumGripper.mRotationCClockwise = Convert.ToBoolean(value);
                            break;
                        case "compressor":
                            Program.vacuumGripper.compressor = Convert.ToBoolean(value);
                            break;
                        case "valve":
                            Program.vacuumGripper.valve = Convert.ToBoolean(value);
                            break;
                        case "state":
                            Program.vacuumGripper.state = (VG_State)Convert.ToInt16(value);
                            break;
                        case "any_motor_running":
                            Program.vacuumGripper.any_motor_running = Convert.ToBoolean(value);
                            break;
                        case "cX":
                            Program.vacuumGripper.cX = Convert.ToInt16(value);
                            break;
                        case "cY":
                            Program.vacuumGripper.cY = Convert.ToInt16(value);
                            break;
                        case "cZ":
                            Program.vacuumGripper.cZ = Convert.ToInt16(value);
                            break;
                        case "bIdx":
                            Program.vacuumGripper.bIdx = Convert.ToInt16(value);
                            break;
                        case "wIdx":
                            Program.vacuumGripper.wIdx = Convert.ToInt16(value);
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Fehler!");
                            Console.ResetColor();
                            break;
                    }
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fehler!");
                    Console.ResetColor();
                    break;
            }


        }

        public static void write(string nodeId, object value)
        {
            Console.WriteLine(nodeId + " " + value.ToString());


        }

        public void Write_node(string nodeId, object value)
        {
            try
            {
                WriteValue valueToWrite = new WriteValue();

                valueToWrite.NodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application." + nodeId;
                valueToWrite.AttributeId = 13;
                valueToWrite.Value.Value =value;
                valueToWrite.Value.StatusCode = StatusCodes.Good;
                valueToWrite.Value.ServerTimestamp = DateTime.MinValue;
                valueToWrite.Value.SourceTimestamp = DateTime.MinValue;

                WriteValueCollection valuesToWrite = new WriteValueCollection();
                valuesToWrite.Add(valueToWrite);

                // write current value.
                StatusCodeCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                
                session.Write(
                    null,
                    valuesToWrite,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, valuesToWrite);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToWrite);

                if (StatusCode.IsBad(results[0]))
                {
                    throw new ServiceResultException(results[0]);
                }
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        /// <summary>
        /// Read batchJobQueue.(node)
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public string Read_node(string nodeId)
        {
            //NodeId m_nodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application.TC.batchJobQueue." + nodeId;

            DataValue m_value;

            ReadValueId nodeToRead = new ReadValueId();
            nodeToRead.NodeId = "ns=4;s=|var|ECC2250 0.8S 1131.Application." + nodeId;
            nodeToRead.AttributeId = Attributes.Value;

            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();
            nodesToRead.Add(nodeToRead);

            //read current value
            DataValueCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;


            //Session m_session = session;
            session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead, out results, out diagnosticInfos);
            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            m_value = results[0];

            return Utils.Format("{0}", m_value.WrappedValue);
        }

       
        private void Client_KeepAlive(Session sender, KeepAliveEventArgs e)
        {
            if (e.Status != null && ServiceResult.IsNotGood(e.Status))
            {
                Console.WriteLine("{0} {1}/{2}", e.Status, sender.OutstandingRequestCount, sender.DefunctRequestCount);

                if (reconnectHandler == null)
                {
                    Console.WriteLine("--- RECONNECTING ---");
                    reconnectHandler = new SessionReconnectHandler();
                    reconnectHandler.BeginReconnect(sender, ReconnectPeriod * 1000, Client_ReconnectComplete);
                }
            }
        }

        private void Client_ReconnectComplete(object sender, EventArgs e)
        {
            // ignore callbacks from discarded objects.
            if (!Object.ReferenceEquals(sender, reconnectHandler))
            {
                return;
            }

            session = reconnectHandler.Session;
            reconnectHandler.Dispose();
            reconnectHandler = null;

            Console.WriteLine("--- RECONNECTED ---");
        }



        private static void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = autoAccept;
                if (autoAccept)
                {
                    Console.WriteLine("Accepted Certificate: {0}", e.Certificate.Subject);
                }
                else
                {
                    Console.WriteLine("Rejected Certificate: {0}", e.Certificate.Subject);
                }
            }
        }




        private void Init_batches()
        {
            //int amountBatchJobs = Int32.Parse(Read_node("amountBatchJobs"));
            //bool full = Convert.ToBoolean(Read_node("full"));
            //int nextFreePos = Int32.Parse(Read_node("nextFreePos"));

            //Program.batchQueue.AmountBatchJobs = amountBatchJobs;
            //Program.batchQueue.Full = full;
            //Program.batchQueue.NextFreePos = nextFreePos;
            Program.batchJobQueue.BatchJobs = new List<BatchJob>();
            //Program.batchJobQueue.Id = 1;

            //batchJobQueues.Add(new BatchJobQueuestruct(amountBatchJobs, new List<BatchJobstruct>(), full, nextFreePos));



            for (int i = 1; i <= Program.maxBatchJobs; i++)//Int32.Parse(Read_node("amountBatchJobs")); i++)
            {
                //int batchSize = Int32.Parse(Read_node("batchJobs[" + i + "].batchSize"));
                //int nextFreeWPos = Int32.Parse(Read_node("batchJobs[" + i + "].nextFreeWPos"));
                //int nextNotProcessedIdx = Int32.Parse(Read_node("batchJobs[" + i + "].nextNotProcessedIdx"));
                //int amountFinishedW = Int32.Parse(Read_node("batchJobs[" + i + "].amountFinishedW"));
                //ExecutionMode mode = (ExecutionMode)Int32.Parse(Read_node("batchJobs[" + i + "].mode"));
                //BatchJobState bjstate = (BatchJobState)Int32.Parse(Read_node("batchJobs[" + i + "].state")); ;

                Program.batchJobQueue.BatchJobs.Add(new BatchJob()
                {
                    Id = i,
                    //BatchSize = batchSize,
                    Workpieces = new List<Workpiece>(),
                    //NextFreeWPos = nextFreePos,
                    //NextNotProcessedIdx = nextNotProcessedIdx,
                    //AmountFinishedW = amountFinishedW,
                    //Mode = mode,
                    //state = bjstate
                });
                //batchJobQueues[0].batchJobs.Add(new BatchJobstruct(batchSize, new List<Workpiecestruct>(), nextFreeWPos, nextNotProcessedIdx, amountFinishedW, mode, bjstate));

                for (int j = 1; j <= Program.maxWorkpieces; j++)//Int32.Parse(Read_node("batchJobs[" + i + "].batchSize")); j++)
                {
                    //String color = Read_node("batchJobs[" + i + "].workpieces[" + j + "].color");
                    //int linePos = Int32.Parse(Read_node("batchJobs[" + i + "].workpieces[" + j + "].linePos"));
                    //int sortingLinePos = Int32.Parse(Read_node("batchJobs[" + i + "].workpieces[" + j + "].sortingLinePos"));
                    //W_State wstate = (W_State)Int32.Parse(Read_node("batchJobs[" + i + "].workpieces[" + j + "].state"));
                    //RackPos startPos = (RackPos)Int32.Parse(Read_node("batchJobs[" + i + "].workpieces[" + j + "].startPos"));
                    //RackPos endPos = (RackPos)Int32.Parse(Read_node("batchJobs[" + i + "].workpieces[" + j + "].endPos"));
                    //bool furEnabled = Convert.ToBoolean(Read_node("batchJobs[" + i + "].workpieces[" + j + "].format.furEnabled"));
                    //bool sawEnabled = Convert.ToBoolean(Read_node("batchJobs[" + i + "].workpieces[" + j + "].format.sawEnabled"));
                    //bool colorCheckEnabled = Convert.ToBoolean(Read_node("batchJobs[" + i + "].workpieces[" + j + "].format.colorCheckEnabled"));
                    //bool ejectEnabled = Convert.ToBoolean(Read_node("batchJobs[" + i + "].workpieces[" + j + "].format.ejectEnabled"));
                    //int furDuration = Int32.Parse(Read_node("batchJobs[" + i + "].workpieces[" + j + "].format.furDuration"));
                    //int sawDuration = Int32.Parse(Read_node("batchJobs[" + i + "].workpieces[" + j + "].format.sawDuration"));

                    //batchJobQueues[0].batchJobs[i - 1].workpieces.Add(new Workpiecestruct(color, linePos, sortingLinePos, wstate, startPos, endPos, new Formatstruct(furEnabled, sawEnabled, colorCheckEnabled, ejectEnabled, furDuration, sawDuration)));
                    Program.batchJobQueue.BatchJobs[i - 1].Workpieces.Add(new Workpiece
                    {
                        Id = j,
                        //Color = color,
                        //LinePos = linePos,
                        //SortingLinePos = sortingLinePos,
                        //State = wstate,
                        //StartPos = startPos,
                        //EndPos = endPos,
                        Formate = new List<Format>
                        {
                            new Format
                            {
                                //Id = j,
                                //FurEnabled = furEnabled,
                                //SawEnabled = sawEnabled,
                                //ColorCheckEnabled = colorCheckEnabled,
                                //EjectEnabled = ejectEnabled,
                                //FurDuration = furDuration,
                                //SawDuration = sawDuration
                            }
                        }

                    });


                }
            }

        }
    }
}
