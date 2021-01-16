using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BMSNameSpace
{
    public class TCPClient
    {
        MainPage rootPage = MainPage.Current;
        private byte AlarmCode(int index)
        {
            byte alarmCode = 0;
            if (rootPage.alarmData[index].R_Upper) alarmCode += Constants.R_UPPER_CODE;
            if (rootPage.alarmData[index].V_Upper) alarmCode += Constants.V_UPPER_CODE;
            if (rootPage.alarmData[index].V_Lower) alarmCode += Constants.V_LOWER_CODE;
            if (rootPage.alarmData[index].E_Upper) alarmCode += Constants.E_UPPER_CODE;
            if (rootPage.alarmData[index].E_Lower) alarmCode += Constants.E_LOWER_CODE;
            if (rootPage.alarmData[index].T_Upper) alarmCode += Constants.T_UPPER_CODE;
            if (rootPage.alarmData[index].T_Lower) alarmCode += Constants.T_LOWER_CODE;
            if (rootPage.measurementData[index].STATUS_STRUCT.presented) alarmCode += Constants.PRESENTED_CODE;

            return alarmCode;
        }
        public String ToJSONRepresentation(MeasurementDataPackage dataPackage)
        {
            StringBuilder sb = new StringBuilder();
            JsonWriter jw = new JsonTextWriter(new StringWriter(sb));

            jw.Formatting = Formatting.Indented;
            jw.WriteStartObject();
            jw.WritePropertyName("date");
            DateTime dtime = new DateTime(dataPackage.timeTicks);
            jw.WriteValue(dtime.ToString("yyyy-MM-dd HH:mm:ss"));
            jw.WritePropertyName("Status");
            jw.WriteValue(dataPackage.status);
            jw.WritePropertyName("System");
            jw.WriteStartObject();
            jw.WritePropertyName("SiteName");
            jw.WriteValue(rootPage.systemName);
            jw.WritePropertyName("Block");
            jw.WriteValue(rootPage.totalKit.ToString("N0"));
            jw.WritePropertyName("String");
            jw.WriteValue(rootPage.totalStr.ToString("N0"));
            jw.WritePropertyName("Unit");
            jw.WriteValue(rootPage.totalUnit.ToString("N0"));
            jw.WritePropertyName("Address");
            jw.WriteStartObject();
            jw.WritePropertyName("Address16");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].ADDRESS16);
            jw.WritePropertyName("Address24");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].ADDRESS24);
            jw.WritePropertyName("Address32");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].ADDRESS32);
            jw.WritePropertyName("Address40");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].ADDRESS40);
            jw.WriteEndObject();
            jw.WritePropertyName("Config");
            jw.WriteStartObject();
            jw.WritePropertyName("Enable");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.enable);
            jw.WritePropertyName("OperationMode");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.operationMode);
            jw.WritePropertyName("RefreshDuration");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.refreshDuration);
            jw.WritePropertyName("VoltageLevel");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.voltageLevel);
            jw.WritePropertyName("DeviceType");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.deviceType);
            jw.WritePropertyName("CableType");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.cableType);
            jw.WritePropertyName("Capacity");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.blkCapacity);
            jw.WriteEndObject();
            jw.WritePropertyName("Alarm");
            jw.WriteStartObject();
            jw.WritePropertyName("R_Upper");
            jw.WriteValue(rootPage.alarmData[0].NB_R_Upper);
            jw.WritePropertyName("V_Upper");
            jw.WriteValue(rootPage.alarmData[0].NB_V_Upper);
            jw.WritePropertyName("V_Lower");
            jw.WriteValue(rootPage.alarmData[0].NB_V_Lower);
            jw.WritePropertyName("E_Upper");
            jw.WriteValue(rootPage.alarmData[0].NB_E_Upper);
            jw.WritePropertyName("E_Lower");
            jw.WriteValue(rootPage.alarmData[0].NB_E_Lower);
            jw.WritePropertyName("T_Upper");
            jw.WriteValue(rootPage.alarmData[0].NB_T_Upper);
            jw.WritePropertyName("T_Lower");
            jw.WriteValue(rootPage.alarmData[0].NB_T_Lower);
            jw.WritePropertyName("Error");
            jw.WriteValue(rootPage.alarmData[0].NB_Missed);
            jw.WriteEndObject();
            jw.WritePropertyName("Communication");
            jw.WriteStartObject();
            jw.WritePropertyName("InternetAvailable");
            jw.WriteValue(rootPage.internetAvailable ? "Available" : "Unavailable");
            jw.WritePropertyName("WiFiSignal");
            jw.WriteValue(rootPage.wifiSignal);
            jw.WritePropertyName("SSID");
            jw.WriteValue(rootPage.internetSSID);
            jw.WritePropertyName("MobileSignal");
            jw.WriteValue(rootPage.mobileSignal);
            jw.WriteEndObject();
            jw.WriteEndObject();

            jw.WritePropertyName("Units");
            jw.WriteStartArray();
            for (int i = 0; i < Constants.MAXUNIT; i++)
            {
                jw.WriteStartObject();
                jw.WritePropertyName("Name");
                jw.WriteValue(rootPage.unitData[i].NAME);
                jw.WritePropertyName("TotalBlock");
                jw.WriteValue(rootPage.unitData[i].TOTAL_BLOCK);
                jw.WritePropertyName("TotalString");
                jw.WriteValue(rootPage.unitData[i].TOTAL_STRING);
                jw.WritePropertyName("Address");
                jw.WriteValue(rootPage.unitData[i].ADDRESS);
                jw.WritePropertyName("BlkCapacity");
                jw.WriteValue(rootPage.unitData[i].BLK_CAPACITY);
                jw.WritePropertyName("RefreshDuration");
                jw.WriteValue(rootPage.unitData[i].REFRESH_DURATION);
                jw.WritePropertyName("Config");
                jw.WriteStartObject();
                jw.WritePropertyName("Enable");
                jw.WriteValue(rootPage.unitData[i].CONFIG_STRUCT.enable);
                jw.WritePropertyName("OperationMode");
                jw.WriteValue(rootPage.unitData[i].CONFIG_STRUCT.operationMode);
                jw.WritePropertyName("RefreshDuration");
                jw.WriteValue(rootPage.unitData[i].CONFIG_STRUCT.refreshDuration);
                jw.WritePropertyName("VoltageLevel");
                jw.WriteValue(rootPage.unitData[i].CONFIG_STRUCT.voltageLevel);
                jw.WritePropertyName("DeviceType");
                jw.WriteValue(rootPage.unitData[i].CONFIG_STRUCT.deviceType);
                jw.WritePropertyName("CableType");
                jw.WriteValue(rootPage.unitData[i].CONFIG_STRUCT.cableType);
                jw.WritePropertyName("Capacity");
                jw.WriteValue(rootPage.unitData[i].CONFIG_STRUCT.blkCapacity);
                jw.WriteEndObject();
                jw.WritePropertyName("Status");
                jw.WriteStartObject();
                jw.WritePropertyName("TotalActiveBlock");
                jw.WriteValue(rootPage.activeBlkList.Count());
                jw.WritePropertyName("TotalAlarmBlock");
                jw.WriteValue(rootPage.nbAlarmBlk[i]);
                jw.WritePropertyName("ReferenceR");
                jw.WriteValue(rootPage.referenceR[i]);
                jw.WritePropertyName("Events");
                jw.WriteValue(rootPage.eventMessage);
                jw.WriteEndObject();
                jw.WriteEndObject();
            }
            jw.WriteEndArray();

            int unit, str, blk , type;
            unit = str = blk = type = 0;
            jw.WritePropertyName("Blocks");
            jw.WriteStartArray();
            for (int i = 1; i < rootPage.totalKit + 1; i++)
            {
                rootPage.GetId(rootPage.settingData, i, ref unit, ref str, ref blk , ref type);
                if (type == Constants.BMK_TYPE)
                {
                    jw.WriteStartObject();
                    jw.WritePropertyName("Block");
                    jw.WriteValue(blk);
                    jw.WritePropertyName("String");
                    jw.WriteValue(str);
                    jw.WritePropertyName("Unit");
                    jw.WriteValue(unit);
                    jw.WritePropertyName("V0");
                    jw.WriteValue(dataPackage.displayData[i].V0.ToString("N3"));
                    jw.WritePropertyName("V1");
                    //                    jw.WriteValue(dataPackage.displayData[i].V1.ToString("N3"));
                    jw.WriteValue(dataPackage.displayData[i].SOC.ToString("N2"));
                    jw.WritePropertyName("V2");
                    jw.WriteValue(dataPackage.displayData[i].V2.ToString("N3"));
                    jw.WritePropertyName("E");
                    jw.WriteValue(dataPackage.displayData[i].E.ToString("N3"));
                    jw.WritePropertyName("R");
                    jw.WriteValue(dataPackage.displayData[i].R.ToString("N3"));
                    jw.WritePropertyName("T");
                    jw.WriteValue(dataPackage.displayData[i].T.ToString("N2"));
                    jw.WritePropertyName("Config");
                    jw.WriteStartObject();
                    jw.WritePropertyName("Enable");
                    jw.WriteValue(rootPage.settingData[i].CONFIG_STRUCT.enable);
                    jw.WritePropertyName("OperationMode");
                    jw.WriteValue(rootPage.settingData[i].CONFIG_STRUCT.operationMode);
                    jw.WritePropertyName("RefreshDuration");
                    jw.WriteValue(rootPage.settingData[i].CONFIG_STRUCT.refreshDuration);
                    jw.WritePropertyName("VoltageLevel");
                    jw.WriteValue(rootPage.settingData[i].CONFIG_STRUCT.voltageLevel);
                    jw.WritePropertyName("DeviceType");
                    jw.WriteValue(rootPage.settingData[i].CONFIG_STRUCT.deviceType);
                    jw.WritePropertyName("CableType");
                    jw.WriteValue(rootPage.settingData[i].CONFIG_STRUCT.cableType);
                    jw.WritePropertyName("Capacity");
                    jw.WriteValue(rootPage.settingData[i].CONFIG_STRUCT.blkCapacity);
                    jw.WriteEndObject();
                    jw.WritePropertyName("Status");
                    jw.WriteStartObject();
                    jw.WritePropertyName("Presented");
                    jw.WriteValue(rootPage.measurementData[i].STATUS_STRUCT.presented);
                    jw.WritePropertyName("OperationMode");
                    jw.WriteValue(rootPage.measurementData[i].STATUS_STRUCT.operationMode);
                    jw.WritePropertyName("Alarm");
                    jw.WriteValue(AlarmCode(i));
                    jw.WritePropertyName("MissingPacket");
                    jw.WriteValue(rootPage.measurementData[i].STATUS_STRUCT.missingPacket);
                    jw.WritePropertyName("NbTransaction");
                    jw.WriteValue(rootPage.measurementData[i].STATUS_STRUCT.nbTransaction);
                    jw.WritePropertyName("TotalDischargeCycle");
                    jw.WriteValue(rootPage.extendedDataList[i].TotalDischargeCycle);
                    jw.WritePropertyName("TotalDischargeSecond");
                    jw.WriteValue(rootPage.extendedDataList[i].TotalDischargeSecond);
                    jw.WritePropertyName("ExpectedLifeTimeRest");
                    jw.WriteValue(rootPage.extendedDataList[i].ExpectedLifeTimeRest);
                    jw.WriteEndObject();
                    jw.WritePropertyName("Alarm");
                    jw.WriteStartObject();
                    jw.WritePropertyName("R_Upper");
                    jw.WriteValue(rootPage.alarmData[i].NB_R_Upper);
                    jw.WritePropertyName("V_Upper");
                    jw.WriteValue(rootPage.alarmData[i].NB_V_Upper);
                    jw.WritePropertyName("V_Lower");
                    jw.WriteValue(rootPage.alarmData[i].NB_V_Lower);
                    jw.WritePropertyName("E_Upper");
                    jw.WriteValue(rootPage.alarmData[i].NB_E_Upper);
                    jw.WritePropertyName("E_Lower");
                    jw.WriteValue(rootPage.alarmData[i].NB_E_Lower);
                    jw.WritePropertyName("T_Upper");
                    jw.WriteValue(rootPage.alarmData[i].NB_T_Upper);
                    jw.WritePropertyName("T_Lower");
                    jw.WriteValue(rootPage.alarmData[i].NB_T_Lower);
                    jw.WritePropertyName("Error");
                    jw.WriteValue(rootPage.alarmData[i].NB_Missed);
                    jw.WriteEndObject();
                    jw.WriteEndObject();
                }
            }
            jw.WriteEndArray();
            jw.WriteEndObject();
            return sb.ToString();
        }
        public String SystemToJSONRepresentation()
        {
            StringBuilder sb = new StringBuilder();
            JsonWriter jw = new JsonTextWriter(new StringWriter(sb));
            MeasurementDataPackage dataPackage = new MeasurementDataPackage();
            dataPackage = rootPage.mDataPackageList.Last();

            jw.Formatting = Formatting.Indented;
            jw.WriteStartObject();
            jw.WritePropertyName("date");
            jw.WriteValue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            jw.WritePropertyName("System");
            jw.WriteStartObject();
            jw.WritePropertyName("SiteName");
            jw.WriteValue(rootPage.systemName);
            jw.WritePropertyName("Block");
            jw.WriteValue(rootPage.totalKit.ToString("N0"));
            jw.WritePropertyName("String");
            jw.WriteValue(rootPage.totalStr.ToString("N0"));
            jw.WritePropertyName("Unit");
            jw.WriteValue(rootPage.totalUnit.ToString("N0"));
            jw.WritePropertyName("Address");
            jw.WriteStartObject();
            jw.WritePropertyName("Address16");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].ADDRESS16);
            jw.WritePropertyName("Address24");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].ADDRESS24);
            jw.WritePropertyName("Address32");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].ADDRESS32);
            jw.WritePropertyName("Address40");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].ADDRESS40);
            jw.WriteEndObject();
            jw.WritePropertyName("Config");
            jw.WriteStartObject();
            jw.WritePropertyName("Enable");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.enable);
            jw.WritePropertyName("OperationMode");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.operationMode);
            jw.WritePropertyName("RefreshDuration");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.refreshDuration);
            jw.WritePropertyName("VoltageLevel");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.voltageLevel);
            jw.WritePropertyName("DeviceType");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.deviceType);
            jw.WritePropertyName("CableType");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.cableType);
            jw.WritePropertyName("Capacity");
            jw.WriteValue(rootPage.settingData[Constants.SYSTEM_INDEX].CONFIG_STRUCT.blkCapacity);
            jw.WriteEndObject();
            jw.WritePropertyName("Alarm");
            jw.WriteStartObject();
            jw.WritePropertyName("R_Upper");
            jw.WriteValue(rootPage.alarmData[0].NB_R_Upper);
            jw.WritePropertyName("V_Upper");
            jw.WriteValue(rootPage.alarmData[0].NB_V_Upper);
            jw.WritePropertyName("V_Lower");
            jw.WriteValue(rootPage.alarmData[0].NB_V_Lower);
            jw.WritePropertyName("E_Upper");
            jw.WriteValue(rootPage.alarmData[0].NB_E_Upper);
            jw.WritePropertyName("E_Lower");
            jw.WriteValue(rootPage.alarmData[0].NB_E_Lower);
            jw.WritePropertyName("T_Upper");
            jw.WriteValue(rootPage.alarmData[0].NB_T_Upper);
            jw.WritePropertyName("T_Lower");
            jw.WriteValue(rootPage.alarmData[0].NB_T_Lower);
            jw.WritePropertyName("Error");
            jw.WriteValue(rootPage.alarmData[0].NB_Missed);
            jw.WriteEndObject();
            jw.WriteEndObject();
            jw.WriteEndObject();
            return sb.ToString();
        }
        public async Task<bool> TcpSender()
        {
            bool result = false;
            if (rootPage.internetAvailable)
            {
                try
                {
                    if (rootPage.outstandingQueue.Count() > 1)                   // There is a lost one       
                    {
                        foreach (MeasurementDataPackage dataPackage in rootPage.outstandingQueue)
                        {
                            dataPackage.status = Constants.PACKAGE_STATUS_LOST;
                        }
                    }
                    if (rootPage.outstandingQueue.Count() != 0)
                    {
                        bool sendingResult = false;
                        Queue<MeasurementDataPackage> tempoOutstandingQueue = new Queue<MeasurementDataPackage>();
                        foreach (MeasurementDataPackage dataPackage in rootPage.outstandingQueue)
                        {
                            string jsonStringw = ToJSONRepresentation(dataPackage);
                            sendingResult = await rootPage.bsm.sendBatteryDataToServerAsync(jsonStringw);
                            if (!sendingResult) tempoOutstandingQueue.Enqueue(dataPackage);
                        }
                        rootPage.outstandingQueue.Clear();
                        if (tempoOutstandingQueue.Count != 0)
                        {
                            foreach (MeasurementDataPackage dataPackage in tempoOutstandingQueue)
                            {
                                rootPage.outstandingQueue.Enqueue(dataPackage);
                            }
                        }
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    result = false;
                }
            }
            else
            {
                rootPage.UpdateStatusPanel("Internet connection lost." + rootPage.outstandingQueue.Count() + " packages outstanding", NotifyType.WarningMessage);
                result = false;
            }
            return result;
        }
        public bool TcpSenderRecord(MeasurementDataPackage dataPackage)
        {
            bool result = false;
            try
            {
                string jsonStringw = ToJSONRepresentation(dataPackage);
                _ = rootPage.bsm.sendBatteryDataToServerAsync(jsonStringw);
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                result = false;
            }
            return result;
        }
    }
}
