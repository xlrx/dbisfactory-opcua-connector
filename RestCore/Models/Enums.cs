using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestCore
{

    public enum ExitCode : int
    {
        Ok = 0,
        ErrorCreateApplication = 0x11,
        ErrorDiscoverEndpoints = 0x12,
        ErrorCreateSession = 0x13,
        ErrorBrowseNamespace = 0x14,
        ErrorCreateSubscription = 0x15,
        ErrorMonitoredItem = 0x16,
        ErrorAddSubscription = 0x17,
        ErrorRunning = 0x18,
        ErrorNoKeepAlive = 0x30,
        ErrorInvalidCommandLine = 0x100
    };
    /// <summary>
    /// Modes of execution of a BatchJob
    /// </summary>
    public enum ExecutionMode
    {
        single = 0,
        multi = 1,
        sorting = 2
    }
    /// <summary>
    /// Positions in high rack. Used for HR navigation
    /// </summary>
    public enum RackPos
    {
        NULL = 0,
        RACK_11 = 1,
        RACK_12 = 2,
        RACK_13 = 3,
        RACK_21 = 4,
        RACK_22 = 5,
        RACK_23 = 6,
        RACK_31 = 7,
        RACK_32 = 8,
        RACK_33 = 9,

        CONVEYOR = 10,

        UP = 11,
        DOWN = 12
    }
    /// <summary>
    /// States in which a BatchJob can be
    /// </summary>
    public enum BatchJobState
    {
        /// <summary>
        /// NULL always expresses that the object has not been initialized yet
        /// </summary>
        NULL = 0,
        /// <summary>
        /// BatchJob has been initialized and is waiting to be executed
        /// </summary>
        WAITING = 1,
        /// <summary>
        /// BatchJob has been started and is being executed now
        /// </summary>
        STARTED = 2,
        /// <summary>
        /// BatchJob has been paused during execution
        /// </summary>
        PAUSED = 3,
        /// <summary>
        /// BatchJob has been aborted. This may happen at any time
        /// </summary>
        ABORTED = 4,
        /// <summary>
        /// BatchJob has been finished
        /// </summary>
        FINISHED = 5
    }

    /// <summary>
    /// States in which the station FUR (Furnace) can be. The station is composed of a burner, succer, rotary desk and saw.
    /// </summary>
    public enum FUR_State
    {
        /// <summary>
        /// NULL always expresses that the Object has not been initialized yet
        /// </summary>
        NULL = 0,

        /// <summary>
        /// used in single mode
        /// FUR is ready to take a new workpiece
        /// </summary>
        FUR_WAITING_FOR_W = 1,
        /// <summary>
        /// Workpiece passed the lightbarrier at waggon
        /// </summary>
        FUR_LB_PASSED = 2,

        /// <summary>
        /// used in single and multi mode
        ///  Wagon is inside of burner
        /// </summary>
        FUR_WAGON_INSIDE = 3,
        /// <summary>
        /// Furnace is finished with buring the workpiece
        /// </summary>
        FUR_BURNED = 4,
        /// <summary>
        ///  Wagon is outside of burner
        /// </summary>
        FUR_WAGON_OUTSIDE = 5,
        /// <summary>
        /// used in single mode
        /// furnace succer is at furnace
        /// </summary>
        FUR_SUCCER_AT_FUR = 6,
        /// <summary>
        /// furnace succer picks up workpiece
        /// </summary>
        FUR_SUCCER_DOWN_AT_FUR = 7,
        /// <summary>
        /// furnace succer picks up workpiece
        /// </summary>
        FUR_SUCCER_UP_AT_FUR = 8,
        /// <summary>
        /// furnace succer on the way to rotary desk
        /// </summary>
        FUR_SUCCER_TO_ROTARY_DESK = 9,
        /// <summary>
        /// furnace succer at rotary desk
        /// </summary>
        FUR_SUCCER_AT_ROTARY_DESK = 10,
        /// <summary>
        /// furnace succer puts down workpiece
        /// </summary>
        FUR_SUCCER_DOWN_AT_RD = 11,
        /// <summary>
        /// furnace succer puts down workpiece
        /// </summary>
        FUR_SUCCER_UP_AT_RD = 12,
        /// <summary>
        /// saw has been finished
        /// </summary>
        FUR_SAW_FINISHED = 13,
        /// <summary>
        /// saw has been skipped	
        /// </summary>	
        FUR_SKIP_SAW = 14,
        /// <summary>
        /// station has finished processing this workpiece
        /// </summary>
        FUR_W_FINISHED_BACK_TO_DEFAULT = 15
    }

    /// <summary>
    /// States in which the station HR (High Rack) can be.
    /// </summary>
    public enum HR_State
    {
        /// <summary>
        /// NULL always expresses that the Object has not been initialized yet
        /// </summary>
        NULL = 0,

        /// <summary>
        /// used in single mode
        /// </summary>
        AT_DEFAULT_POS = 1,
        /// <summary>
        /// used in single mode
        /// </summary>
        FINISHED_SINGLE_MODE = 2,
        /// <summary>
        /// used in single mode
        /// </summary>
        AT_CONVEYOR_BOX_FULL = 25,
        /// <summary>
        /// used in single mode
        /// </summary>
        AT_CONVEYOR_BOX_EMPTY = 26,

        /// <summary>
        /// used in multi mode
        /// </summary>
        WAIT_FOR_VG_TO_BRING_W = 8,
        /// <summary>
        /// used in multi mode
        /// also used in single mode
        /// </summary>
        GET_W = 9,
        /// <summary>
        /// used in multi mode
        /// </summary>
        GET_BOX = 10,
        /// <summary>
        /// used in multi mode
        /// </summary>
        STORE_W = 20,
        /// <summary>
        /// used in multi mode
        /// </summary>
        STORE_BOX = 19,
        /// <summary>
        /// used in multi mode
        /// </summary>
        WAIT_FOR_VG_TO_PICK_UP_W = 11,
        /// <summary>
        /// used in multi mode
        /// </summary>
        READY = 12,

        /// <summary>
        /// used in sorting mode
        /// </summary>
        FINISHED_SORTING_MODE = 40,
        /// <summary>
        /// used in sorting mode
        /// </summary>
        SORTING = 41
    }

    /// <summary>
    /// States in which the station SL (Sorting Line) can be.
    /// </summary>
    public enum SL_State
    {
        /// <summary>
        /// NULL always expresses that the object has not been initialized yet
        /// </summary>
        NULL = 0,

        /// <summary>
        /// SL is ready to process a new workpiece
        /// </summary>
        WAITING_FOR_W = 1,
        /// <summary>
        /// Lightbarrier at SL entry has been passed by a workpiece
        /// </summary>
        W_AT_LB = 2,

        W_AT_COLORCHECK = 3,
        COLORCHECK_FINISHED = 4,
        W_READY_FOR_SORTING = 5,
        /// <summary>
        /// if no color was detected, the workpiece will be thrown out to trash 
        /// </summary>
        THROW_OUT = 6,

        SORTING = 7,
        WAITING_FOR_VG_TO_PICK_UP_W	 = 8,
        EJECT = 9,
        EJECTED = 10,

        /// <summary>
        ///  SL got a workpiece to process it
        /// </summary>
        W_RECEIVED = 11
    }

    /// <summary>
    /// States in which the TC (Task Configurator) can be.
    /// </summary>
    public enum TC_State
    {
        /// <summary>
        /// NULL always expresses that the Object has not been initialized yet
        /// </summary>
        NULL = 0,

        /// <summary>
        /// TC has loaded a new workpiece to HR
        /// </summary>
        W_LOADED = 1,
        /// <summary>
        /// TC is waiting until the workpiece has been processed
        /// </summary>
        WAIT_UNTIL_W_FINISHED = 2,

        /// <summary>
        /// TC is loading a new workpiece
        /// </summary>
        LOAD_NEXT_W = 3
    }

    /// <summary>
    /// States in which the station VG (Vacuum Gripper) can be.
    /// </summary>
    public enum VG_State
    {
        /// <summary>
        ///  NULL always expresses that the object has not been initialized yet
        /// </summary>
        NULL = 0,

        /// <summary>
        /// used in single mode
        /// </summary>
        FROM_HR_TO_EJECT = 6,
        /// <summary>
        /// used in single mode
        /// </summary>
        AT_HR = 30,
        /// <summary>
        /// used in single mode
        /// </summary>
        FROM_HR_TO_FUR = 31,
        /// <summary>
        /// used in single mode
        /// </summary>
        AT_SL_OUTPUT_1 = 32,
        /// <summary>
        /// used in single mode
        /// </summary>
        AT_SL_OUTPUT_2 = 33,
        /// <summary>
        /// used in single mode
        /// </summary>
        AT_SL_OUTPUT_3 = 34,
        /// <summary>
        /// used in single mode
        /// </summary>
        WAIT_FOR_SL = 35,
        /// <summary>
        /// used in single mode
        /// </summary>
        AT_EJECT_1 = 36,
        /// <summary>
        /// used in single mode
        /// </summary>
        AT_EJECT_2 = 37,
        /// <summary>
        /// used in single mode
        /// </summary>
        AT_EJECT_3 = 38,
        /// <summary>
        /// used in single mode
        /// </summary>
        TO_SL_OUTPUT_1 = 39,
        /// <summary>
        /// used in single mode
        /// </summary>
        TO_SL_OUTPUT_2 = 40,
        /// <summary>
        /// used in single mode
        /// </summary>
        TO_SL_OUTPUT_3 = 41,
        /// <summary>
        /// used in single mode
        /// </summary>
        FROM_DEFAULT_POS_TO_HR = 43,
        /// <summary>
        /// used in single mode
        /// </summary>
        GET_W_FROM_HR = 44,
        /// <summary>
        /// used in single mode
        /// </summary>
        ABOVE_SL = 56,
        /// <summary>
        /// used in single mode
        /// </summary>
        FROM_SL_TO_HR = 57,
        /// <summary>
        /// used in single mode
        /// </summary>
        DROPPING_W_TO_HR = 59,

        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_FUR_PUT_W = 8,
        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_FUR_PUT_W_FINISHED = 9,
        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_SL_TAKE_W = 10,
        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_SL_TAKE_W_FINISHED = 11,
        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_HR_PUT_W = 12,
        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_HR_PUT_W_FINISHED = 13,
        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_HR_TAKE_W = 14,
        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_HR_TAKE_W_FINISHED = 15,
        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_EJECT = 16,
        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_EJECT_FINISHED = 17,
        /// <summary>
        /// used in multi mode
        /// </summary>
        TO_SL = 45,
        /// <summary>
        /// ready if VG is at default pos or if VG put down a workpiece at HR --> VG is ready for next workpiece
        /// </summary>
        READY = 18,

        /// <summary>
        /// used in single and multi mode
        /// </summary>
        AT_DEFAULT_POS = 1,
        /// <summary>
        /// used in single and multi mode
        /// </summary>
        TO_DEFAULT_POS = 5
    }

    /// <summary>
    /// States in which a workpiece can be
    /// </summary>
    public enum W_State
    {
        /// <summary>
        /// NULL always expresses that the object has not been initialized yet
        /// </summary>
        NULL = 0,

        /// <summary>
        /// states at HR
        /// </summary>
        AT_RACK_11 = 111,
        /// <summary>
        /// states at HR
        /// </summary>
        AT_RACK_12 = 112,
        /// <summary>
        /// states at HR
        /// </summary>
        AT_RACK_13 = 113,
        /// <summary>
        /// states at HR
        /// </summary>
        AT_RACK_21 = 121,
        /// <summary>
        /// states at HR
        /// </summary>
        AT_RACK_22 = 122,
        /// <summary>
        /// states at HR
        /// </summary>
        AT_RACK_23 = 123,
        /// <summary>
        /// states at HR
        /// </summary>
        AT_RACK_31 = 131,
        /// <summary>
        /// states at HR
        /// </summary>
        AT_RACK_32 = 132,
        /// <summary>
        /// states at HR
        /// </summary>
        AT_RACK_33 = 133,

        /// <summary>
        /// NOT used
        /// </summary>
        FROM_RACK_TO_CONVEYOR = 140,
        /// <summary>
        /// NOT used
        /// </summary>
        AT_CONVEYOR_INSIDE = 141,
        /// <summary>
        /// NOT used
        /// </summary>
        FROM_CONVEYOR_INSIDE_TO_OUTSIDE = 142,
        /// <summary>
        /// NOT used
        /// </summary>
        AT_CONVEYOR_OUTSIDE = 143,
        /// <summary>
        /// NOT used
        /// </summary>
        FROM_CONVEYOR_OUTSIDE_TO_INSIDE = 144,

        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        AT_HR = 200,
        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        FROM_HR_TO_FUR = 201,
        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        FROM_SL_OUTPUT_TO_HR = 202,
        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        FROM_SL_OUTPUT_1_TO_HR = 203,
        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        FROM_SL_OUTPUT_2_TO_HR = 204,
        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        FROM_SL_OUTPUT_3_TO_HR = 205,
        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        FROM_SL_TO_EJECT = 206,
        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        FROM_SL_OUTPUT_1_TO_EJECT = 207,
        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        FROM_SL_OUTPUT_2_TO_EJECT = 208,
        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        FROM_SL_OUTPUT_3_TO_EJECT = 209,
        /// <summary>
        /// states at VG
        /// NOT used
        /// </summary>
        FROM_FUR_TO_HR = 210,

        /// <summary>
        /// states at FUR
        /// </summary>
        AT_FUR_IN_WAGON = 300,
        /// <summary>
        /// states at FUR
        /// </summary>
        AT_FUR_IN_FUR = 301,
        /// <summary>
        /// states at FUR
        /// </summary>
        AT_FUR_ATTACHED_TO_SUCCER = 302,
        /// <summary>
        /// states at FUR
        /// </summary>
        AT_FUR_ON_ROTARY_DESK = 303,
        /// <summary>
        /// states at FUR
        /// </summary>
        AT_FUR_AT_SAW = 304,
        /// <summary>
        /// states at FUR
        /// </summary>
        AT_FUR_ON_CONVEYOR = 305,

        /// <summary>
        /// states at SL
        /// not used
        /// </summary>
        AT_SL_AT_ENTRY_LIGHT_BARRIER = 400,
        /// <summary>
        /// states at SL
        /// not used
        /// </summary>
        AT_SL_AT_COLORCHECK = 401,
        /// <summary>
        /// states at SL
        /// not used
        /// </summary>
        AT_SL_AT_EXIT_LIGHT_BARRIER = 402,
        /// <summary>
        /// states at SL
        /// not used
        /// </summary>
        AT_SL_OUTPUT_1 = 403,
        /// <summary>
        /// states at SL
        /// not used
        /// </summary>
        AT_SL_OUTPUT_2 = 404,
        /// <summary>
        /// states at SL
        /// not used
        /// </summary>
        AT_SL_OUTPUT_3 = 405,
        /// <summary>
        /// states at SL
        /// not used
        /// </summary>
        AT_SL_OUTPUT_TRASH = 406,


        /// <summary>
        /// states at EJECT point
        /// not used
        /// </summary>
        AT_EJECT_1 = 500,
        /// <summary>
        /// states at EJECT point
        /// not used
        /// </summary>
        AT_EJECT_2 = 501,
        /// <summary>
        /// states at EJECT point
        /// not used
        /// </summary>
        AT_EJECT_3 = 502
    }

}

