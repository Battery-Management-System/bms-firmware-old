using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BMSNameSpace
{
    public class DebugTraceListener : ITraceListener
    {
        public void Receive(string message)
        {
            Debug.WriteLine(message);
        }
    }
    public class AblockData
    {
        //[PrimaryKey, AutoIncrement]
        public long timeTicks { get; set; }
        public String data { get; set; }

    }
    public class CSVType
    {
        public string Block { get; set; }                                             // Block Id 
        public string V0 { get; set; }                                              // Block floating voltage measured
        public string E { get; set; }                                               // Block internal voltage calculated
        public string R { get; set; }                                               // Block impedance calculated
        public string T { get; set; }                                               // Block temperature measured
    }
    public class CSVDataType
    {
        public int Block { get; set; }                                             // Block Id 
        public float V0 { get; set; }                                              // Block floating voltage measured
        public float E { get; set; }                                               // Block internal voltage calculated
        public float R { get; set; }                                               // Block impedance calculated
        public float T { get; set; }                                               // Block temperature measured
    }

    public class CSVHeaderType
    {
        public string Description { get; set; }                                    // Field description  
        public string Value { get; set; }                                          // Value 
    }
    enum JOB
    {
        DIAGNOS TIC = 0,
        DATA_PROCESS,
        OVER_FLOW,
        SERVER_DISCONNECTED,
        DAILY_EMAILING,
        REQUEST_EMAILING,
    };
    public class JobDataType
    {
        public int JobCode { get; set; }                                             // Enum JOB 
        public int NbTrial { get; set; }                                             // Number of trial done  
        public int MaxNbTrial { get; set; }                                          // Max Trial 
        public int TrialDuration { get; set; }                                       // Duration in second for next trial 
    }
    enum OPERATION
    {
        NORMAL = 0,
        SETTING,
        CALIBRATION,
        ENUMERATION,
    };
    public enum STATECTRL
    {
        STATE_CONTROL_IDLE = 1000,
        STATE_CONTROL_SETTING = 1001,
        STATE_CONTROL_NEXTPAGE = 1002,
        STATE_CONTROL_BACKPAGE = 1003,
    }
    public enum SELWINDOWS
    {
        SELWINDOWS_MONITOR = 0,
        SELWINDOWS_GRAPH,
        SELWINDOWS_EVENTS,
        SELWINDOWS_ALARMS,
        SELWINDOWS_SETTINGS,
        SELWINDOWS_COMMUNICATION,
        SELWINDOWS_EXIT,
        SELWINDOWS_STATUS ,
        SELWINDOWS_LEAKAGE

    }
    public enum SELGRAPHCHART
    {
        SELCHART_VOLTAGE_V0 = 0,
        SELCHART_VOLTAGE_E,
        SELCHART_RESISTANCE,
        SELCHART_TEMPERATURE,
        SELCHART_SOC,
        SELCHART_VOLTAGE_VE,
        SELCHART_ALL
    }
    public enum SELPERCENTAGECHART
    {
        SELCHART_VOLTAGE = 0,
        SELCHART_REGISTER,
        SELCHART_TEMPERATURE,
    }
    public enum BITWISE
    {
        BIT_0 = 0x0001,
        BIT_1 = 0x0002,
        BIT_2 = 0x0004,
        BIT_3 = 0x0008,
        BIT_4 = 0x0010,
        BIT_5 = 0x0020,
        BIT_6 = 0x0040,
        BIT_7 = 0x0080,
        BIT_8 = 0x0100,
        BIT_9 = 0x0200,
        BIT_10 = 0x0400,
        BIT_11 = 0x0800,
        BIT_12 = 0x1000,
        BIT_13 = 0x2000,
        BIT_14 = 0x4000,
        BIT_15 = 0x8000
    }
    enum DATA_INDEX
    {
        R = 0,
        V,
        E,
        T
    };
    enum RANGE_INDEX
    {
        MIN = 0,
        MAX
    };
    enum VOLTAGE_LEVEL
    {
        LEVEL_12000 = 0,
        LEVEL_6000,
        LEVEL_2000,
        LEVEL_1200
    };
    enum VOLTAGE_TYPE
    {
        OPEN = 0,
        FLOATING,
        BOOST,
        END
    };
    enum CULTURE
    {
        EN_US = 0,
        EN_GB,
        EN_CA,
        FR_CA,
        FR_FR,
        IT_IT,
        DE_DE,
        ES_ES,
        EN_SG,
        VI_VN
    };
    public class ConfigDataType
    {
        public bool enable;                                             // Blk enable
        public byte operationMode;                                      // Mode : 0:Normal 1: Setting 2: Calibration 3:Enumeration    
        public byte refreshDuration;                                    // Duration : 0:10s 1:30s 2:60s 3:300s 4:600s 5: 1800s 6:3600 7: 10800s
        public byte voltageLevel;                                       // Voltage level : (BMK) 0:16V/12V 1: 6V 2:2V 3: 1.2V  (SMK & DC) 0:220V 1: 110V 2:48V 3: 24V
        public byte deviceType;                                         // Device type : 0:BMK 1: SMK 2:DC 3: reserved                                             
        public byte cableType;                                          // Cable type : 0:120mm 1:250mm 2: reserved 3:reserved
        public byte blkCapacity;                                        // Block capacity
        private ushort config;
        public ushort Config                                           // Consolidate config word
        {
            get
            {
                config = 0;
                if (enable) config += 1;
                config += (ushort)((int)operationMode << (int)1);
                config += (ushort)((int)refreshDuration << (int)3);
                config += (ushort)((int)voltageLevel << (int)6);
                config += (ushort)((int)deviceType << (int)8);
                config += (ushort)((int)cableType << (int)10);
                config += (ushort)((int)blkCapacity << (int)12);
                return config;
            }
            set
            {
                if ((value & (int)BITWISE.BIT_0) != 0) enable = true;
                operationMode = (byte)(((int)value & (int)(BITWISE.BIT_1 | BITWISE.BIT_2)) >> (int)1);
                refreshDuration = (byte)(((int)value & (int)(BITWISE.BIT_3 | BITWISE.BIT_4 | BITWISE.BIT_5)) >> (int)3);
                voltageLevel = (byte)(((int)value & (int)(BITWISE.BIT_6 | BITWISE.BIT_7)) >> (int)6);
                deviceType = (byte)(((int)value & (int)(BITWISE.BIT_8 | BITWISE.BIT_9)) >> (int)8);
                cableType = (byte)(((int)value & (int)(BITWISE.BIT_10 | BITWISE.BIT_11)) >> (int)10);
                blkCapacity = (byte)(((int)value & (int)(BITWISE.BIT_12 | BITWISE.BIT_13 | BITWISE.BIT_14 | BITWISE.BIT_15)) >> (int)12);
            }
        }
    }

    public class StatusDataType
    {
        public bool presented;                                          // Presented & active
        public byte operationMode;                                      // Mode : 0:Normal 1: Setting 2: Calibration 3:Enumeration
        public byte alarm;                                              // Alarm : 0:Normal 1: SettingUnavailable 2: LostPacket 3: OutOfThreshold    
        public byte missingPacket;                                      // Number of consequent packet missing
        public byte nbTransaction;                                      // Number of total transaction done                                             
        private ushort status;                                          // Consolidate status word
        public ushort Status                                            // Consolidate status word access
        {
            get
            {
                status = 0;
                if (presented) status += 1;
                status += (ushort)(operationMode << 1);
                status += (ushort)(alarm << 3);
                status += (ushort)(missingPacket << 5);
                status += (ushort)(nbTransaction << 8);
                return status;
            }
            set
            {
                if ((value % 2) != 0) presented = true;
                else presented = false;
                operationMode = (byte)((value & (ushort)(BITWISE.BIT_1 | BITWISE.BIT_2)) >> 1);
                alarm = (byte)((value & (ushort)(BITWISE.BIT_3 | BITWISE.BIT_4)) >> 3);
                missingPacket = (byte)((value & (ushort)(BITWISE.BIT_5 | BITWISE.BIT_6 | BITWISE.BIT_7)) >> 5);
                nbTransaction = (byte)((value & 0xff00) >> 8);
                status = value;
                /*                if ((value & (ushort)BITWISE.BIT_0) != 0) presented = true;
                                else presented = false;
                                operationMode = (byte)((value & (ushort)(BITWISE.BIT_1 | BITWISE.BIT_2)) >> 1);
                                alarm = (byte)((value & (ushort)(BITWISE.BIT_3 | BITWISE.BIT_4)) >> 3);
                                missingPacket = (byte)((value & (ushort)(BITWISE.BIT_5 | BITWISE.BIT_6 | BITWISE.BIT_7)) >> 5);
                                nbTransaction = (byte)((value & 0xff00) >> 8);
                */
            }
        }

    }
    [StructLayout(LayoutKind.Explicit)]
    public struct PROCESSFLAGSTYPE
    {
        [FieldOffset(0)]
        public bool startupFinished;                      //flag to indicate system is starting
        [FieldOffset(1)]
        public bool shutdownRequest;                      //flag to indicate shutdown request    
        [FieldOffset(2)]
        public bool updateDataAvailable;                  //flag to indicate data received is measurement data 
        [FieldOffset(3)]
        public bool settingDataAvailable;                 //flag to indicate data received is setting data 
        [FieldOffset(4)]
        public bool calibDataAvailable;                   //flag to indicate data received is calibration data 
        [FieldOffset(5)]
        public bool enumDataAvailable;                   //flag to indicate data received is enumeration data 
        [FieldOffset(6)]
        public bool transferDataError;                    //flag to indicate transfer data error
        [FieldOffset(7)]
        public bool settingDataModified;                   //flag to indicate setting data modified
        [FieldOffset(8)]
        public bool settingDataError;                      //flag to indicate setting data error
        [FieldOffset(9)]
        public bool settingDataSaveRequired;               //flag to indicate setting data need to be saved
        [FieldOffset(10)]
        public bool settingsWindowRefreshRequired;         //flag to indicate setting window must be refreshed
        [FieldOffset(11)]
        public bool alarmsWindowRefreshRequired;          //flag to indicate alarm window must be refreshed
        [FieldOffset(12)]
        public bool eventsWindowRefreshRequired;          //flag to indicate event window must be refreshed
        [FieldOffset(13)]
        public bool graphWindowRefreshRequired;          //flag to indicate graph window must be refreshed
        [FieldOffset(14)]
        public bool monitorWindowRefreshRequired;        //flag to indicate monitor window must be refreshed
        [FieldOffset(15)]
        public bool dataDispatchingRequired;             //flag to indicate data disptaching to database and other services required after finishing refresh data from PIC
        [FieldOffset(16)]
        public bool dataProcessingRequired;             //flag to indicate data processing required after dispatching
        [FieldOffset(17)]
        public bool alarmProcessingRequired;             //flag to indicate alarm analysing required after processing data
        [FieldOffset(18)]
        public bool pivotUpdateRequired;                //flag to indicate update pivot windows required after finishing data processing
        [FieldOffset(19)]
        public bool TCPProcessingRequired;              //flag to indicate TCP service required
        [FieldOffset(20)]
        public bool uart3GProcessingRequired;           //flag to indicate 3G service required
        [FieldOffset(21)]
        public bool configDataAvailable;                //flag to indicate config data loaded 
        [FieldOffset(22)]
        public bool submitToSendEmail;                  //flag to indicate email should be sent 
        [FieldOffset(23)]
        public bool submitToReset;                      //flag to indicate reset required 
        [FieldOffset(24)]
        public bool MasterInProgress;                   //flag to indicate Master refresh period
        [FieldOffset(25)]
        public bool LoraInProgress;                     //flag to indicate Lora refresh period
        [FieldOffset(26)]
        public bool RFInProgress;                       //flag to indicate RF refresh period
        [FieldOffset(27)]
        public bool forceToSendEmail;                   //flag to indicate enmail forced to send by user 
        [FieldOffset(28)]
        public bool refreshDurationChanged;             //flag to indicate system refresh period changed
        [FieldOffset(29)]
        public bool sendEmailRequestOnNewDay;           //flag to indicate report email must be sent on new day 
        [FieldOffset(30)]
        public bool sendEmailSuccessfully;              //flag to indicate report email was sent successfully 
        [FieldOffset(31)]
        public bool processInProgress;                  //flag to indicate main process after receiving SPI data in progress 
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct COMMFLAGSTYPE
    {
        [FieldOffset(0)]
        public bool MobileNetworkAvailable;             //flag to indicate 3G module ready & connected to Internet
        [FieldOffset(1)]
        public bool TCPAvailable;                       //flag to indicate Wifi or Ethernet connected to Internet    
        [FieldOffset(2)]
        public bool LORAAvailable;                      //flag to indicate LORA module available & ready for transfering data 
        [FieldOffset(3)]
        public bool SPIAvailable;                       //flag to indicate SPI master ready 
    }

    public class ExtendedDataType
    {
        public float V0Max;                                           // BMK:Max floating voltage/SMK:Max string voltage/LMK:Max positive voltage 
        public float V0Min;                                           // BMK:Min floating voltage/SMK:Min string voltage/LMK:Min positive voltage  
        public float EMax;                                            // BMK:Max internal voltage/SMK:Max calculated string voltage/LMK:Max negative voltage 
        public float EMin;                                            // BMK:Min internal voltage/SMK:Min calculated string voltage/LMK:Min negative voltage  
        public float RMax;                                            // BMK:Max R/SMK:Max R in string/LMK:Min positive R 
        public float RMin;                                            // BMK:Min R/SMK:Min R in string/LMK:Min negative R 
        public float RAvr;                                            // BMK:Average R/SMK:Average R in string/LMK:Average min R 
        public float RMode;                                           // BMK:R value that occurs most often//SMK:R value that occurs most often in string/LMK:R value that occurs most often. 
        public float RDeviation;                                      // BMK/SMK/LMK: Standard deviation of R 
        public float TMax;                                            // BMK/SMK/LMK: Max temperature 
        public float SOCMin;                                          // BMK:Min SOC
        public int TotalDischargeSecond;                              // BMK:Total discharge time in second   
        public int TotalDischargeCycle;                               // BMK:Total discharge cycle   
        public float ExpectedLifeTimeRest;                            // BMK:Rest of Life time expected    
        public void Reset()
        {
            V0Max = V0Min = EMax = EMin = RMax = RMin = RAvr = RMode = RDeviation = TMax = 0;
        }
    }
    public class DisplayDataType
    {
        public long timeTicks;                                        // Time stamp  
        public float V0;                                              // Block floating voltage measured
        public float V1;                                              // Block 1st step voltage measured
        public float V2;                                              // Block 2nd voltage measured
        public float E;                                               // Block internal voltage calculated
        public float R;                                               // Block impedance calculated
        public float T;                                               // Block temperature measured
        public float SOC;                                             // Block SOC derived
        public bool Copy(DisplayDataType dest)
        {
            dest.timeTicks = timeTicks;
            dest.V0 = V0;
            dest.V1 = V1;
            dest.V2 = V2;
            dest.E = E;
            dest.R = R;
            dest.T = T;
            dest.SOC = SOC;
            return true;
        }
        public bool Retrieve(DisplayDataType source)
        {
            timeTicks = source.timeTicks;
            V0 = source.V0;
            V1 = source.V1;
            V2 = source.V2;
            E = source.E;
            R = source.R;
            T = source.T;
            SOC = source.SOC;
            return true;
        }
    }

    public class LeakageDataType
    {
        public long timeTicks;                                        // Time stamp  
        public float Vp;                                              // Positive branch voltage measured
        public float Vn;                                              // Negative branch voltage measured
        public float Rfp;                                             // Positive fault resistance calculated
        public float Rfn;                                             // Negative fault resistance calculated
        public float If1;                                             // First differential current measured
        public float If2;                                             // Second differential current measured
        public float H;                                               // Humidity per centage

        public bool Copy(LeakageDataType dest)
        {
            dest.timeTicks = timeTicks;
            dest.Vp = Vp;
            dest.Vn = Vn;
            dest.Rfp = Rfp;
            dest.Rfn = Rfn;
            dest.If1 = If1;
            dest.If2 = If2;
            dest.H = H;
            return true;
        }
        public bool Retrieve(LeakageDataType source)
        {
            timeTicks = source.timeTicks;
            Vp = source.Vp;
            Vn = source.Vn;
            Rfp = source.Rfp;
            Rfn = source.Rfn;
            If1 = source.If1;
            If2 = source.If2;
            H = source.H;
            return true;
        }
    }
    public class AlarmDataType
    {
        public bool FromBegining;                                      // Accumulate from begining 
        public bool FromLastHour;                                      // Accumulate from last hour 
        public bool Reported;                                          // Status reported or not 
        public bool R_Upper;                                           // Status of  over R upper threshold 
        public bool V_Upper;                                           // Status of over V upper threshold 
        public bool V_Lower;                                           // Status of lover V lover threshold 
        public bool E_Upper;                                           // Status of over E upper threshold 
        public bool E_Lower;                                           // Status of lover E lover threshold 
        public bool T_Upper;                                           // Status of over T upper threshold 
        public bool T_Lower;                                           // Status of lover T lover threshold 
        public uint NB_R_Upper;                                        // Number of over R upper threshold 
        public uint NB_V_Upper;                                        // Number of over V upper threshold 
        public uint NB_V_Lower;                                        // Number of lover V lover threshold 
        public uint NB_E_Upper;                                        // Number of over E upper threshold 
        public uint NB_E_Lower;                                        // Number of lover E lover threshold 
        public uint NB_T_Upper;                                        // Number of over T upper threshold 
        public uint NB_T_Lower;                                        // Number of lover T lover threshold 
        public uint NB_Missed;                                         // Number of missing refresh cycle 
        public uint NB_Alert;                                          // Number of sending alert 
        public void Clear()
        {
            FromBegining = Reported = true;
            FromLastHour = false;
            NB_R_Upper = NB_V_Upper = NB_V_Lower = NB_E_Upper = NB_E_Lower = NB_T_Upper = NB_T_Lower = NB_Missed = NB_Alert = 0;
        }
    }
    public class MeasurementDataType
    {
        public ushort V0;                                              // Block floating voltage measured
        public ushort V1;                                              // Block 1st step voltage measured
        public ushort V2;                                              // Block 2nd step voltage measured
        public ushort T;                                               // Block temperature measured
        public ushort sV1;                                             // SW1 drop voltage 
        public ushort sV2;                                             // SW2 drop voltage
        public ushort E;
        public StatusDataType STATUS_STRUCT = new StatusDataType();

        public bool Copy(byte[] dest)
        {
            dest[0] = Convert.ToByte(Convert.ToUInt16(V0) & 0x00FF);
            dest[1] = Convert.ToByte(Convert.ToUInt16(V0) >> 8);
            dest[2] = Convert.ToByte(Convert.ToUInt16(V1) & 0x00FF);
            dest[3] = Convert.ToByte(Convert.ToUInt16(V1) >> 8);
            dest[4] = Convert.ToByte(Convert.ToUInt16(V2) & 0x00FF);
            dest[5] = Convert.ToByte(Convert.ToUInt16(V2) >> 8);
            dest[6] = Convert.ToByte(Convert.ToUInt16(T) & 0x00FF);
            dest[7] = Convert.ToByte(Convert.ToUInt16(T) >> 8);
            dest[8] = Convert.ToByte(Convert.ToUInt16(sV1) & 0x00FF);
            dest[9] = Convert.ToByte(Convert.ToUInt16(sV1) >> 8);
            dest[10] = Convert.ToByte(Convert.ToUInt16(sV2) & 0x00FF);
            dest[11] = Convert.ToByte(Convert.ToUInt16(sV2) >> 8);
            dest[12] = Convert.ToByte(Convert.ToUInt16(E) & 0x00FF);
            dest[13] = Convert.ToByte(Convert.ToUInt16(E) >> 8);
            dest[14] = Convert.ToByte(Convert.ToUInt16(STATUS_STRUCT.Status) & 0x00FF);
            dest[15] = Convert.ToByte(Convert.ToUInt16(STATUS_STRUCT.Status) >> 8);
            return true;
        }
        public bool Retrieve(byte[] source)
        {
            V0 = (ushort)(Convert.ToUInt16(source[0]) + Convert.ToUInt16(source[1]) * 256);
            V1 = (ushort)(Convert.ToUInt16(source[2]) + Convert.ToUInt16(source[3]) * 256);
            V2 = (ushort)(Convert.ToUInt16(source[4]) + Convert.ToUInt16(source[5]) * 256);
            T = (ushort)(Convert.ToUInt16(source[6]) + Convert.ToUInt16(source[7]) * 256);
            sV1 = (ushort)(Convert.ToUInt16(source[8]) + Convert.ToUInt16(source[9]) * 256);
            sV2 = (ushort)(Convert.ToUInt16(source[10]) + Convert.ToUInt16(source[11]) * 256);
            E = (ushort)(Convert.ToUInt16(source[12]) + Convert.ToUInt16(source[13]) * 256);
            STATUS_STRUCT.Status = (ushort)(Convert.ToUInt16(source[14]) + Convert.ToUInt16(source[15]) * 256);
            return true;
        }
    };
    public class MeasurementDataVType
    {
        public ushort V0;                                              // 1st second voltage
        public ushort V1;                                              // 2nd second voltage
        public ushort V2;                                              // 3rd second voltage
        public ushort V3;                                              // 4th second voltage 
        public ushort V4;                                              // 5th second voltage
        public ushort V5;                                              // 6th second voltage
        public ushort V6;                                              // spare
        public StatusDataType STATUS_STRUCT = new StatusDataType();

        public bool Copy(ref byte[] dest)
        {
            dest[0] = Convert.ToByte(Convert.ToUInt16(V0) & 0x00FF);
            dest[1] = Convert.ToByte(Convert.ToUInt16(V0) >> 8);
            dest[2] = Convert.ToByte(Convert.ToUInt16(V1) & 0x00FF);
            dest[3] = Convert.ToByte(Convert.ToUInt16(V1) >> 8);
            dest[4] = Convert.ToByte(Convert.ToUInt16(V2) & 0x00FF);
            dest[5] = Convert.ToByte(Convert.ToUInt16(V2) >> 8);
            dest[6] = Convert.ToByte(Convert.ToUInt16(V3) & 0x00FF);
            dest[7] = Convert.ToByte(Convert.ToUInt16(V3) >> 8);
            dest[8] = Convert.ToByte(Convert.ToUInt16(V4) & 0x00FF);
            dest[9] = Convert.ToByte(Convert.ToUInt16(V4) >> 8);
            dest[10] = Convert.ToByte(Convert.ToUInt16(V5) & 0x00FF);
            dest[11] = Convert.ToByte(Convert.ToUInt16(V5) >> 8);
            dest[12] = Convert.ToByte(Convert.ToUInt16(V6) & 0x00FF);
            dest[13] = Convert.ToByte(Convert.ToUInt16(V6) >> 8);
            dest[14] = Convert.ToByte(Convert.ToUInt16(STATUS_STRUCT.Status) & 0x00FF);
            dest[15] = Convert.ToByte(Convert.ToUInt16(STATUS_STRUCT.Status) >> 8);
            return true;
        }
        public bool Copy(ref MeasurementDataVType dest)
        {
            dest.V0 = V0;
            dest.V1 = V1;
            dest.V2 = V2;
            dest.V3 = V3;
            dest.V4 = V4;
            dest.V5 = V5;
            dest.V6 = V6;
            dest.STATUS_STRUCT.Status = STATUS_STRUCT.Status;
            return true;
        }
        public bool Retrieve(byte[] source)
        {
            V0 = (ushort)(Convert.ToUInt16(source[0]) + Convert.ToUInt16(source[1]) * 256);
            V1 = (ushort)(Convert.ToUInt16(source[2]) + Convert.ToUInt16(source[3]) * 256);
            V2 = (ushort)(Convert.ToUInt16(source[4]) + Convert.ToUInt16(source[5]) * 256);
            V3 = (ushort)(Convert.ToUInt16(source[6]) + Convert.ToUInt16(source[7]) * 256);
            V4 = (ushort)(Convert.ToUInt16(source[8]) + Convert.ToUInt16(source[9]) * 256);
            V5 = (ushort)(Convert.ToUInt16(source[10]) + Convert.ToUInt16(source[11]) * 256);
            V6 = (ushort)(Convert.ToUInt16(source[12]) + Convert.ToUInt16(source[13]) * 256);
            STATUS_STRUCT.Status = (ushort)(Convert.ToUInt16(source[14]) + Convert.ToUInt16(source[15]) * 256);
            return true;
        }
        public bool Retrieve(MeasurementDataVType source)
        {
            V0 = source.V0;
            V1 = source.V1;
            V2 = source.V2;
            V3 = source.V3;
            V4 = source.V4;
            V5 = source.V5;
            V6 = source.V6;
            STATUS_STRUCT.Status = source.STATUS_STRUCT.Status;
            return true;
        }
    };
    public class VarianceDataType
    {
        public ushort time;                                           // moment of variance
        public bool isDischarging;                                    // type of variance
        public bool isFloating;                                       // type of variance
        public bool isBoosting;                                       // type of variance
        public bool isEndOfDischarge;                                 // type of variance
        public bool isDecreasing;                                     // gradient of voltage - decreasing 
        public bool isIncreasing;                                     // gradient of voltage - increasing
        public ushort V;                                              // voltage at variance
    };
    public class UnitDataType
    {
        public string NAME;                                                    // Unit alias name
        public byte TOTAL_BLOCK;                                               // Total Block
        public byte TOTAL_STRING;                                              // Total String
        public byte TOTAL_LEAKAGE;                                             // Total Leakage kit
        public byte TOTAL_COMM;                                                // Total Communication kit
        public byte ADDRESS;                                                   // LSB Address
        public byte BLK_CAPACITY;                                              // Blk capacity
        public byte REFRESH_DURATION;                                          // Refreshing duration
        public byte NB_PAGE;                                                   // Total page refreshed
        public byte CURRENT_PAGE;                                              // Current refreshed page
        public byte REPEAD_PAGE;                                               // Repeated page
        public ushort RECORD_INDEX;                                            // First record index
        public byte ADJUST_INDEX;                                              // Adjusting index
        public ConfigDataType CONFIG_STRUCT = new ConfigDataType();            // Config word    
        public StatusDataType STATUS_STRUCT = new StatusDataType();
        public bool Copy(byte[] dest, byte offset)
        {
            dest[Constants.MAXUNIT * 0 + offset] = TOTAL_BLOCK;
            dest[Constants.MAXUNIT * 1 + offset] = (byte)((TOTAL_STRING) | (REFRESH_DURATION << Constants.REFRESH_POS));
            dest[Constants.MAXUNIT * 2 + offset] = ADDRESS;
            dest[Constants.MAXUNIT * 3 + offset] = BLK_CAPACITY;
            return true;
        }
        public bool Retrieve(byte[] source)
        {
            TOTAL_BLOCK = source[0];
            TOTAL_STRING = (byte)(source[1] & ~Constants.REFRESH_MASK);
            REFRESH_DURATION = (byte)((source[1] & Constants.REFRESH_MASK) >> Constants.REFRESH_POS);
            ADDRESS = source[2];
            BLK_CAPACITY = source[3];
            return true;
        }
    }

    public class SettingDataType
    {
        public byte BLOCK;                                                     // Block enum
        public byte STRING;                                                    // String enum
        public byte UNIT;                                                      // Unit enum
        public ushort ADDRESS16;                                               // First two LSB Physical address
        public byte ADDRESS24;                                                 // Third byte Physical address
        public byte ADDRESS32;                                                 // Fourth byte Physical address
        public byte ADDRESS40;                                                 // Fith byte Physical address
        public ushort R1;                                                      // 1st step resistor
        public ushort R2;                                                      // 2nd step resistor    
        public ushort VSCALE;                                                  // Voltage scale
        public ConfigDataType CONFIG_STRUCT = new ConfigDataType();            // Config word    
        public bool Copy(byte[] dest)
        {
            dest[0] = BLOCK;
            dest[1] = STRING;
            dest[2] = UNIT;
            dest[3] = Convert.ToByte(Convert.ToUInt16(ADDRESS16) & 0x00FF);
            dest[4] = Convert.ToByte(Convert.ToUInt16(ADDRESS16) >> 8);
            dest[5] = ADDRESS24;
            dest[6] = ADDRESS32;
            dest[7] = ADDRESS40;
            dest[8] = Convert.ToByte(Convert.ToUInt16(R1) & 0x00FF);
            dest[9] = Convert.ToByte(Convert.ToUInt16(R1) >> 8);
            dest[10] = Convert.ToByte(Convert.ToUInt16(R2) & 0x00FF);
            dest[11] = Convert.ToByte(Convert.ToUInt16(R2) >> 8);
            dest[12] = Convert.ToByte(Convert.ToUInt16(VSCALE) & 0x00FF);
            dest[13] = Convert.ToByte(Convert.ToUInt16(VSCALE) >> 8);
            dest[14] = Convert.ToByte(Convert.ToUInt16(CONFIG_STRUCT.Config) & 0x00FF);
            dest[15] = Convert.ToByte(Convert.ToUInt16(CONFIG_STRUCT.Config) >> 8);
            return true;
        }
        public bool Retrieve(byte[] source)
        {
            BLOCK = source[0];
            STRING = source[1];
            UNIT = source[2];
            ADDRESS16 = (ushort)(Convert.ToUInt16(source[3]) + Convert.ToUInt16(source[4]) * 256);
            ADDRESS24 = source[5];
            ADDRESS32 = source[6];
            ADDRESS40 = source[7];
            R1 = (ushort)(Convert.ToUInt16(source[8]) + Convert.ToUInt16(source[9]) * 256);
            R2 = (ushort)(Convert.ToUInt16(source[10]) + Convert.ToUInt16(source[11]) * 256);
            VSCALE = (ushort)(Convert.ToUInt16(source[12]) + Convert.ToUInt16(source[13]) * 256);
            CONFIG_STRUCT.Config = (ushort)(Convert.ToUInt16(source[14]) + Convert.ToUInt16(source[15]) * 256);
            return true;
        }
        public bool Copy(SettingDataType dest)
        {
            dest.BLOCK = BLOCK;
            dest.STRING = STRING;
            dest.UNIT = UNIT;
            dest.ADDRESS16 = ADDRESS16;
            dest.ADDRESS24 = ADDRESS24;
            dest.ADDRESS32 = ADDRESS32;
            dest.ADDRESS40 = ADDRESS40;
            dest.R1 = R1;
            dest.R2 = R2;
            dest.VSCALE = VSCALE;
            dest.BLOCK = BLOCK;
            dest.CONFIG_STRUCT = CONFIG_STRUCT;
            return true;
        }
        public bool Retrieve(SettingDataType source)
        {
            BLOCK = source.BLOCK;
            STRING = source.STRING;
            UNIT = source.UNIT;
            ADDRESS16 = source.ADDRESS16;
            ADDRESS24 = source.ADDRESS24;
            ADDRESS32 = source.ADDRESS32;
            ADDRESS40 = source.ADDRESS40;
            R1 = source.R1;
            R2 = source.R2;
            VSCALE = source.VSCALE;
            BLOCK = source.BLOCK;
            CONFIG_STRUCT = source.CONFIG_STRUCT;
            return true;
        }
        public bool Compare(SettingDataType dest)
        {
            if ((dest.BLOCK == BLOCK) &&
                (dest.STRING == STRING) &&
                (dest.UNIT == UNIT) &&
                (dest.ADDRESS16 == ADDRESS16) &&
                (dest.ADDRESS24 == ADDRESS24) &&
                (dest.ADDRESS32 == ADDRESS32) &&
                (dest.ADDRESS40 == ADDRESS40) &&
                (dest.R1 == R1) &&
                (dest.R2 == R2) &&
                (dest.VSCALE == VSCALE) &&
                (dest.BLOCK == BLOCK) &&
                (dest.CONFIG_STRUCT == CONFIG_STRUCT))
                return true;
            else return false;
        }
    }
    public class MeasurementDataPackage
    {
        public long timeTicks;
        public byte status;
        public List<DisplayDataType> displayData = new List<DisplayDataType>();
        public bool Copy(MeasurementDataPackage dest)
        {
            dest.timeTicks = timeTicks;
            dest.status = status;
            dest.displayData.Clear();
            foreach (DisplayDataType displayDataRecord in displayData)
            {
                DisplayDataType tempDisplayData = new DisplayDataType();
                tempDisplayData.Copy(displayDataRecord);
                dest.displayData.Add(tempDisplayData);
            }
            return true;
        }
        public bool Copy(List<DisplayDataType> dest)
        {
            dest.Clear();
            foreach (DisplayDataType displayDataRecord in displayData)
            {
                DisplayDataType tempDisplayData = new DisplayDataType();
                tempDisplayData.Copy(displayDataRecord);
                dest.Add(tempDisplayData);
            }
            return true;
        }
        public bool Retrieve(MeasurementDataPackage source)
        {
            timeTicks = source.timeTicks;
            status = source.status;
            foreach (DisplayDataType displayDataRecord in source.displayData)
            {
                DisplayDataType tempDisplayData = new DisplayDataType();
                tempDisplayData.Retrieve(displayDataRecord);
                displayData.Add(tempDisplayData);
            }
            return true;
        }
        public bool Retrieve(List<DisplayDataType> source)
        {
            foreach (DisplayDataType displayDataRecord in source)
            {
                DisplayDataType tempDisplayData = new DisplayDataType();
                tempDisplayData.Retrieve(displayDataRecord);
                displayData.Add(tempDisplayData);
            }
            return true;
        }
    }
    public class SMSConfigType
    {
        public string number;                                          // Phone number
        public bool generalAlarm;
        public bool serviceAlarm;
        public bool criticalAlarm;
        public bool sendReport;
        public bool acceptRequire;
    }
    public class EmailConfigType
    {
        public string address;                                          // Email address
        public bool generalAlarm;
        public bool serviceAlarm;
        public bool criticalAlarm;
        public bool sendReport;
        public bool acceptRequire;
    }
    public class SMTPConfigType
    {
        public string server;                                           // SMTP server 
        public string user;                                             // SMTP user
        public string password;                                         // SMTP password
        public int port;                                                // SMTP port
        public byte connect;                                            // Connect type
    }
    public class CommSettingType
    {
        public string Mobile_APN;
        public string Mobile_Name;
        public string Mobile_Password;
        public string TCP_IP3;
        public string TCP_IP2;
        public string TCP_IP1;
        public string TCP_IP0;
        public int TCP_Port;
        public uint RS485_Port;
        public uint RS485_Baudrate;
        public List<SMSConfigType> SMSList = new List<SMSConfigType>();
        public List<EmailConfigType> EmailList = new List<EmailConfigType>();
        public SMTPConfigType SMTPConfig = new SMTPConfigType();
        public string WIFI_SSID;
        public string WIFI_Password;

    }
    public class AlarmSettingType
    {
        public byte R_Upper;
        public byte V_Upper;
        public byte V_Lower;
        public byte E_Upper;
        public byte E_Lower;
        public byte T_Upper;
        public byte T_Lower;
    }
    public class AxisDataType
    {
        public float max;                                              // Axis max value
        public float min;                                              // Axis min value
        public float interval;                                         // Axis interval 
        public int nbXItem;                                            // Number of X items 
        public int XLenght;                                            // Lenght o X axis 
    }
    public class GraphRrefreshType
    {
        public bool V0;                                              // V0 graph refreshed
        public bool E;                                              // E graph refreshed
        public bool VE;                                              // VE graph refreshed
        public bool R;                                              // R graph refreshed
        public bool T;                                              // T graph refreshed
        public bool SOC;                                            // SOC graph refreshed
        public void Clear()
        {
            V0 = E = VE = R = T = SOC = false;
        }
    }
    public class BlockTagType
    {
        public int unit;                                              // Unit of represented button
        public int str;                                               // String of represented button
        public int blk;                                               // Block of represented button
        public int record;                                           // Index of block in displayData list
    }
    public class DataPoint
    {
        public float Value1
        {
            get;
            set;
        }
        public float Value2
        {
            get;
            set;
        }
        public float Value3
        {
            get;
            set;
        }
        public float Value4
        {
            get;
            set;
        }
        public String DT
        {
            get;
            set;
        }
    }
    public class receivedEventArgs : EventArgs
    {
        public int Total;
        public List<MeasurementDataType> measurementData;
        public List<DisplayDataType> displayData;
        public receivedEventArgs(int iTotal, List<MeasurementDataType> iMeasurementData, List<DisplayDataType> iDisplayData)
        {
            Total = iTotal;
            measurementData = iMeasurementData;
            displayData = iDisplayData;
        }
    }
    public class configEventArgs : EventArgs
    {
        public List<SettingDataType> settingData;
        public configEventArgs(List<SettingDataType> iSettingData)
        {
            settingData = iSettingData;
        }
    }

    public class savingDataType
    {
        public Queue<MeasurementDataPackage> saveData = new Queue<MeasurementDataPackage>();
        public List<int> activeList = new List<int>();
        public List<AlarmDataType> alarmList = new List<AlarmDataType>();
    }



}
