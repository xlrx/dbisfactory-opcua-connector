using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using RestCore.Models;
using System.Collections.Generic;

namespace RestCore
{
    public class Program
    {
        /// <summary>
        /// BatchJobQueue
        /// </summary>
        public static BatchJobQueue batchJobQueue;
        /// <summary>
        /// SortingLine
        /// </summary>
        public static SortingLine sortingLine;
        /// <summary>
        /// VacuumGripper
        /// </summary>
        public static VacuumGripper vacuumGripper;
        /// <summary>
        /// HighRack
        /// </summary>
        public static HighRack highRack;
        /// <summary>
        /// Furnance
        /// </summary>
        public static Furnance furnance;
        /// <summary>
        /// TaskConfigurator
        /// </summary>
        public static TaskConfigurator taskConfigurator;
        /// <summary>
        /// How much BatchJobs are Init in Codesys
        /// </summary>
        public static int maxBatchJobs = 6;
        /// <summary>
        /// How much Workpieces are Init in Codesys
        /// </summary>
        public static int maxWorkpieces = 5;

        /// <summary>
        /// Standart Client
        /// </summary>
        public static MyClient client = new MyClient("opc.tcp://169.254.255.86:4840", false, Timeout.Infinite);

        /// <summary>
        /// Start here
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(string[] args)
        {

            batchJobQueue = new BatchJobQueue();

            #region testdaten   
            /*
            {
                AmountBatchJobs = 2,
                Full = false,
                NextFreePos = 3,
                BatchJobs = new List<BatchJob>()
                {
                    new BatchJob()
                    {
                        BatchSize = 2,
                        Workpieces = new List<Workpiece>
                        {
                            new Workpiece()
                            {
                                Color = "unknown",
                                LinePos = 1,
                                SortingLinePos = 1,
                                State = W_State.AT_RACK_11,
                                StartPos = RackPos.RACK_11,
                                EndPos = RackPos.RACK_11,
                                Formate = new List<Format>
                                {
                                    new Format()
                                    {
                                        FurEnabled = true,
                                        SawEnabled = true,
                                        ColorCheckEnabled = true,
                                        EjectEnabled = true,
                                        FurDuration = 100,
                                        SawDuration = 100
                                    }
                                }
                            },
                            new Workpiece()
                            {
                                Color = "unknown",
                                LinePos = 2,
                                SortingLinePos = 2,
                                State = W_State.AT_RACK_22,
                                StartPos = RackPos.RACK_22,
                                EndPos = RackPos.RACK_22,
                                Formate = new List<Format>
                                {
                                    new Format()
                                    {
                                        FurEnabled = true,
                                        SawEnabled = true,
                                        ColorCheckEnabled = true,
                                        EjectEnabled = true,
                                        FurDuration = 200,
                                        SawDuration = 200
                                    }
                                }
                            }
                        },
                        NextFreeWPos = 3,
                        AmountFinishedW = 1,
                        NextNotProcessedIdx = 0,
                        Mode = ExecutionMode.multi,
                        state = BatchJobState.WAITING
                    },
                    new BatchJob()
                    {
                        BatchSize = 2,
                        Workpieces = new List<Workpiece>()
                        {
                            new Workpiece()
                            {
                                Color = "unknown",
                                LinePos = 3,
                                SortingLinePos = 3,
                                State = W_State.AT_RACK_33,
                                StartPos = RackPos.RACK_33,
                                EndPos = RackPos.RACK_33,
                                Formate = new List<Format>
                                {
                                    new Format()
                                    {
                                        FurEnabled = true,
                                        SawEnabled = true,
                                        ColorCheckEnabled = true,
                                        EjectEnabled = true,
                                        FurDuration = 300,
                                        SawDuration = 300
                                    }
                                }
                            },
                            new Workpiece()
                            {
                                Color = "unknown",
                                LinePos = 4,
                                SortingLinePos = 4,
                                State = W_State.AT_RACK_13,
                                StartPos = RackPos.RACK_13,
                                EndPos = RackPos.RACK_13,
                                Formate = new List<Format>
                                {
                                    new Format()
                                    {
                                        FurEnabled = true,
                                        SawEnabled = true,
                                        ColorCheckEnabled = true,
                                        EjectEnabled = true,
                                        FurDuration = 400,
                                        SawDuration = 400
                                    }
                                }
                            }
                        },
                        NextFreeWPos = 3,
                        AmountFinishedW = 1,
                        NextNotProcessedIdx = 0,
                        Mode = ExecutionMode.multi,
                        state = BatchJobState.WAITING
                    }
                }
            };*/
            #endregion


            sortingLine = new SortingLine();
            vacuumGripper = new VacuumGripper();
            highRack = new HighRack();
            furnance = new Furnance();
            taskConfigurator = new TaskConfigurator();

            Thread thread = new Thread(new ThreadStart(client.Run));
            thread.Start();
            

            BuildWebHost(args).Run();

            

            return (int)MyClient.ExitCode;
        }

        /// <summary>
        /// Build Webhost
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            .UseUrls("http://localhost:10000/")
                .Build();
        

    }


}
