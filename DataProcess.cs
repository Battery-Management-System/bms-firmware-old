using System;
using System.Collections.Generic;
using System.Linq;

namespace BMSNameSpace
{
    public class DataProcess
    {
        MainPage rootPage = MainPage.Current;
        public DataProcess()
        {
        }

        public void StatisticCal(List<float> List, ref float avr, ref float mode, ref float deviation)
        {
            avr = List.Average();
            var most = (from x in List
                        group x by x into grp
                        orderby grp.Count() descending
                        select grp.Key).First();
            mode = most;
            double average = List.Average();
            double sumOfSquaresOfDifferences = List.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / List.Count());
            deviation = (float)sd;
        }
        public bool DataPackageProcessing(List<MeasurementDataPackage> DataPackageList)
        {
            MeasurementDataPackage dataPackage = new MeasurementDataPackage();
            int nbData = DataPackageList.Count();
            List<float> RList = new List<float>();

            if (nbData > Constants.MIN_EVAL_DATA)
            {
                foreach (ExtendedDataType extDat in rootPage.extendedDataList) extDat.Reset();
                for (int j = nbData - Constants.MIN_EVAL_DATA; j < nbData; j++)
                {
                    dataPackage = DataPackageList[j];
                    foreach (int i in rootPage.beenActiveBlkList)                           // Excluding the system record
                    {
                        if (rootPage.settingData[i].CONFIG_STRUCT.deviceType == Constants.BMK_TYPE)                 // BMK data processing
                        {
                            if (dataPackage.displayData[i].V0 > rootPage.extendedDataList[i].V0Max) rootPage.extendedDataList[i].V0Max = dataPackage.displayData[i].V0;
                            if ((dataPackage.displayData[i].V0 < rootPage.extendedDataList[i].V0Min) || (rootPage.extendedDataList[i].V0Min == 0)) rootPage.extendedDataList[i].V0Min = dataPackage.displayData[i].V0;
                            if (dataPackage.displayData[i].E > rootPage.extendedDataList[i].EMax) rootPage.extendedDataList[i].EMax = dataPackage.displayData[i].E;
                            if ((dataPackage.displayData[i].E < rootPage.extendedDataList[i].EMin) || (rootPage.extendedDataList[i].EMin == 0)) rootPage.extendedDataList[i].EMin = dataPackage.displayData[i].E;
                            if (dataPackage.displayData[i].R > rootPage.extendedDataList[i].RMax) rootPage.extendedDataList[i].RMax = dataPackage.displayData[i].R;
                            if ((dataPackage.displayData[i].R < rootPage.extendedDataList[i].RMin) || (rootPage.extendedDataList[i].RMin == 0)) rootPage.extendedDataList[i].RMin = dataPackage.displayData[i].R;
                            if (dataPackage.displayData[i].T > rootPage.extendedDataList[i].TMax) rootPage.extendedDataList[i].TMax = dataPackage.displayData[i].T;
                            if (dataPackage.displayData[i].SOC < rootPage.extendedDataList[i].SOCMin) rootPage.extendedDataList[i].SOCMin = dataPackage.displayData[i].SOC;
                        }
                        else if (rootPage.settingData[i].CONFIG_STRUCT.deviceType == Constants.SMK_TYPE)
                        {
                            if (Constants.SMK_ADDRESS40_DEFAULT.Contains(rootPage.settingData[i].ADDRESS40))        // SMK data processing 
                            {

                            }
                            if (Constants.LMK_ADDRESS40_DEFAULT.Contains(rootPage.settingData[i].ADDRESS40))        // LMK data processing
                            {

                            }
                        }

                    }
                }
                dataPackage = DataPackageList.Last();
                if (rootPage.beenActiveBlkList.Count() != 0)
                {
                    foreach (int i in rootPage.beenActiveBlkList)
                    {
                        if (rootPage.settingData[i].CONFIG_STRUCT.deviceType == Constants.BMK_TYPE)
                        {
                            for (int j = nbData - Constants.MIN_EVAL_DATA; j < nbData; j++)
                                RList.Add(DataPackageList[j].displayData[i].R);
                            if (RList.Count() != 0)
                            {
                                StatisticCal(RList, ref rootPage.extendedDataList[i].RAvr, ref rootPage.extendedDataList[i].RMode, ref rootPage.extendedDataList[i].RDeviation);
                                RList.Clear();
                            }
                            if (rootPage.extendedDataList[i].V0Max > rootPage.extendedDataList[0].V0Max) rootPage.extendedDataList[0].V0Max = rootPage.extendedDataList[i].V0Max;
                            if ((rootPage.extendedDataList[i].V0Min < rootPage.extendedDataList[0].V0Min) || (rootPage.extendedDataList[0].V0Min == 0)) rootPage.extendedDataList[0].V0Min = rootPage.extendedDataList[i].V0Min;
                            if (rootPage.extendedDataList[i].EMax > rootPage.extendedDataList[0].EMax) rootPage.extendedDataList[0].EMax = rootPage.extendedDataList[i].EMax;
                            if ((rootPage.extendedDataList[i].EMin < rootPage.extendedDataList[0].EMin) || (rootPage.extendedDataList[0].EMin == 0)) rootPage.extendedDataList[0].EMin = rootPage.extendedDataList[i].EMin;
                            if (rootPage.extendedDataList[i].RMax > rootPage.extendedDataList[0].RMax) rootPage.extendedDataList[0].RMax = rootPage.extendedDataList[i].RMax;
                            if ((rootPage.extendedDataList[i].RMin < rootPage.extendedDataList[0].RMin) || (rootPage.extendedDataList[0].RMin == 0)) rootPage.extendedDataList[0].RMin = rootPage.extendedDataList[i].RMin;
                            if (rootPage.extendedDataList[i].TMax > rootPage.extendedDataList[0].TMax) rootPage.extendedDataList[0].TMax = rootPage.extendedDataList[i].TMax;
                            if (rootPage.extendedDataList[i].SOCMin < rootPage.extendedDataList[0].SOCMin) rootPage.extendedDataList[0].SOCMin = rootPage.extendedDataList[i].SOCMin;
                        }
                    }
                    foreach (int i in rootPage.beenActiveBlkList)
                    {
                        if (rootPage.settingData[i].CONFIG_STRUCT.deviceType == Constants.BMK_TYPE)
                        {
                            RList.Add(rootPage.extendedDataList[i].RAvr);
                        }
                    }
                    if (RList.Count() != 0)
                    {
                        StatisticCal(RList, ref rootPage.extendedDataList[0].RAvr, ref rootPage.extendedDataList[0].RMode, ref rootPage.extendedDataList[0].RDeviation);
                        RList.Clear();
                    }
                }
                return true;
            }
            else return false;
        }
        private bool isValueBelow(ushort value, ushort refValue, ushort window)
        {
            if ((value > Constants.MIN_AD_VALUE) && (value < Constants.MAX_AD_VALUE) &&
                (refValue > Constants.MIN_AD_VALUE + window) && (refValue < Constants.MAX_AD_VALUE))
                return ((value < refValue - window));
            else return false;
        }
        private bool isValueExcess(ushort value, ushort refValue, ushort window)
        {
            if ((value > Constants.MIN_AD_VALUE) && (value < Constants.MAX_AD_VALUE) &&
                (refValue < Constants.MAX_AD_VALUE - window))
                return ((value > refValue + window));
            else return false;
        }
        private bool isValueOutWindow(ushort value, ushort refValue, ushort window)
        {
            if ((value > Constants.MIN_AD_VALUE) && (value < Constants.MAX_AD_VALUE) &&
                (refValue > Constants.MIN_AD_VALUE + window) && (refValue < Constants.MAX_AD_VALUE))
                return ((value > refValue + window) || (value < refValue - window));
            else return false;
        }
        private bool isValueInWindow(ushort value, ushort refValue, ushort window)
        {
            if ((value > Constants.MIN_AD_VALUE) && (value < Constants.MAX_AD_VALUE) &&
                (refValue > Constants.MIN_AD_VALUE + window) && (refValue < Constants.MAX_AD_VALUE - window))
                return ((value < refValue + window) && (value > refValue - window));
            else return false;
        }
        private bool checkVarianceType(ushort moment, ushort value, ushort refValue, ref List<VarianceDataType> varianceKitData, byte voltageLevel)
        {
            bool varianceDeteted = false;
            if (isValueOutWindow(value, refValue, Constants.DV))
            {
                varianceDeteted = true;
                VarianceDataType varianceRecord = new VarianceDataType() { time = moment, V = value, isDischarging = false, isFloating = false, isBoosting = false, isDecreasing = false, isIncreasing = false };
                if (isValueBelow(value, refValue, Constants.DV_PROGRESSIVE))
                {
                    varianceRecord.isDecreasing = true;
                    varianceRecord.isIncreasing = false;
                }
                else varianceRecord.isDecreasing = false;
                if (isValueExcess(value, refValue, Constants.DV_PROGRESSIVE))
                {
                    varianceRecord.isDecreasing = false;
                    varianceRecord.isIncreasing = true;
                }
                else varianceRecord.isIncreasing = false;
                if (isValueInWindow(value, Constants.OPEN_VOLTAGE_LEVEL_AD[voltageLevel], Constants.DV_PROGRESSIVE))
                {
                    varianceRecord.isDischarging = true;
                    varianceRecord.isFloating = false;
                    varianceRecord.isBoosting = false;
                    varianceRecord.isEndOfDischarge = false;
                }
                if (isValueInWindow(value, Constants.FLOATING_VOLTAGE_LEVEL_AD[voltageLevel], Constants.DV_PROGRESSIVE))
                {
                    varianceRecord.isDischarging = false;
                    varianceRecord.isFloating = true;
                    varianceRecord.isBoosting = false;
                    varianceRecord.isEndOfDischarge = false;
                }
                if (isValueInWindow(value, Constants.BOOST_VOLTAGE_LEVEL_AD[voltageLevel], Constants.DV_PROGRESSIVE))
                {
                    varianceRecord.isDischarging = false;
                    varianceRecord.isFloating = false;
                    varianceRecord.isBoosting = true;
                    varianceRecord.isEndOfDischarge = false;
                }
                if (isValueInWindow(value, Constants.END_VOLTAGE_LEVEL_AD[voltageLevel], Constants.DV_PROGRESSIVE))
                {
                    varianceRecord.isDischarging = false;
                    varianceRecord.isFloating = false;
                    varianceRecord.isBoosting = false;
                    varianceRecord.isEndOfDischarge = true;
                }
                varianceKitData.Add(varianceRecord);
            }
            return varianceDeteted;
        }

        public bool varianceDetect(MeasurementDataVType currentV, MeasurementDataVType lastV, ref List<VarianceDataType> varianceKitData, byte voltageLevel)
        {
            bool detected = false;
            varianceKitData.Clear();
            detected |= checkVarianceType(0, currentV.V0, lastV.V5, ref varianceKitData, voltageLevel);
            detected |= checkVarianceType(1, currentV.V1, currentV.V0, ref varianceKitData, voltageLevel);
            detected |= checkVarianceType(2, currentV.V2, currentV.V1, ref varianceKitData, voltageLevel);
            detected |= checkVarianceType(3, currentV.V3, currentV.V2, ref varianceKitData, voltageLevel);
            detected |= checkVarianceType(4, currentV.V4, currentV.V3, ref varianceKitData, voltageLevel);
            detected |= checkVarianceType(5, currentV.V5, currentV.V4, ref varianceKitData, voltageLevel);
            return detected;
        }
        public void DataCalcul(ushort currentPage, List<SettingDataType> setting, byte[] raw, ref List<DisplayDataType> result, ref List<LeakageDataType> leakageResult, ref List<MeasurementDataType> measurement, ref List<MeasurementDataVType> measurementV, ref List<MeasurementDataVType> measurementLastV, ref List<List<VarianceDataType>> varianceData)
        {
            byte[] tempo = new byte[Constants.RECORDWIDTH];
            float V0, V1, V2, E, R, T, Kad, sV1, sV2, V00, I01, I02, V10, I11, I12 , Vp1 , Ve1 , Vp2, Ve2, Rfp , Rfn;
            float R1, R2;
            int index = currentPage * rootPage.nbKitperPage;
            int byteIndex = Constants.DATARXOFFSET;
            int lenght;
            if (index + rootPage.nbKitperPage < rootPage.totalKit + 1) lenght = rootPage.nbKitperPage;                                  // Threre is nbKit+1 record being transferred including system records
            else lenght = rootPage.totalKit + 1 - index;
            for (int i = index; i < index + lenght; i++)
            {
                Array.Copy(raw, byteIndex, tempo, 0, Constants.RECORDWIDTH);
                measurement[i].STATUS_STRUCT.Status = (ushort)(Convert.ToUInt16(tempo[14]) + Convert.ToUInt16(tempo[15]) * 256);
                if (measurement[i].STATUS_STRUCT.presented)
                {
                    if (measurement[i].STATUS_STRUCT.alarm == Constants.R_DATA)
                    {
                        measurement[i].Retrieve(tempo);

                        if (setting[i].CONFIG_STRUCT.deviceType == Constants.BMK_TYPE)
                        {
                            //                            sV1 = (float)(raw[8 + byteIndex] + ((raw[9 + byteIndex]) * 256));
                            //                            sV2 = (float)(raw[10 + byteIndex] + ((raw[11 + byteIndex]) * 256));
                            V0 = (float)measurement[i].V0;
                            V1 = (float)measurement[i].V1;
                            V2 = (float)measurement[i].V2;
                            T = (float)measurement[i].T;
                            sV1 = (float)measurement[i].sV1;
                            sV2 = (float)measurement[i].sV2;
                            // Validate the raw value 
                            if ((V0 > Constants.MIN_AD_VALUE) && (V0 < Constants.MAX_AD_VALUE) &&
                                (V1 > Constants.MIN_AD_VALUE) && (V1 < Constants.MAX_AD_VALUE) &&
                                (V2 > Constants.MIN_AD_VALUE) && (V2 < Constants.MAX_AD_VALUE) &&
                                (T > Constants.MIN_TEMP_VALUE) && (T < Constants.MAX_TEMP_VALUE))
                            {           // Calculation in case of 2 step closing
                                R = 0;
                                R1 = setting[i].R1;
                                R2 = setting[i].R2;
                                float dV = V1 - V2;
                                float I2 = (V2 - sV2) / R2;
                                float dI = I2 - dV / R1;
                                R = dV / dI / 10;

                                Kad = (float)setting[i].VSCALE / 10000 / 1000;
                                //                        E = (float)measurement[i].E;
                                E = R * I2 + V2;
                                T /= 16;                                                                      // 25/4/100
                                result[i].V0 = V0 * Kad;
                                result[i].V1 = V1 * Kad;
                                result[i].V2 = V2 * Kad;
                                result[i].E = E * Kad;
                                result[i].R = R;
                                result[i].R -= Constants.CABLE_RESISTANCE[setting[i].CONFIG_STRUCT.cableType];
                                result[i].R -= Constants.XT60_CONTACT_RESISTANCE;
                                result[i].R *= (float)rootPage.unitData[setting[i].UNIT].ADJUST_INDEX / 128f;

                                result[i].T = T;
                                result[i].SOC = Constants.SOC_FACTOR[setting[i].CONFIG_STRUCT.voltageLevel, 1] * result[i].E + Constants.SOC_FACTOR[setting[i].CONFIG_STRUCT.voltageLevel, 0];
                                if (result[i].SOC < 0) result[i].SOC = 0;
                                if (result[i].SOC > 100) result[i].SOC = 100;
                                result[i].timeTicks = DateTime.Now.Ticks;
                            }
                        }
                        else if (setting[i].CONFIG_STRUCT.deviceType == Constants.LMK_TYPE)
                        {
                            Vp1 = (float)measurement[i].V0;
                            Ve1 = (float)measurement[i].V1;
                            Vp2 = (float)measurement[i].V2;
                            T = (float)measurement[i].T;

                            Ve2 = (float)measurement[i].sV1;
                            I11 = (float)measurement[i].sV2;
                            I12 = (float)measurement[i].E;

                            Vp1 -= Constants.ELEVEN_POW;
                            Ve1 -= Constants.ELEVEN_POW;
                            Vp2 -= Constants.ELEVEN_POW;
                            Ve2 -= Constants.ELEVEN_POW;

                            LeakageDataType tempoLeakageData=new LeakageDataType();

                            float Ip, In, Ie , Vp ,Vn , Ve;

                            float VOffset = (float)(rootPage.settingData[i].VSCALE - Constants.FITTHTEEN_POW) / 1000;
                            float I1Offset = (float)(rootPage.settingData[i].R1 - Constants.FITTHTEEN_POW) / 1000;
                            float I2Offset = (float)(rootPage.settingData[i].R2 - Constants.FITTHTEEN_POW) / 1000;
                            Ve = Ve1 * Constants.LMK_V_KAD;
                            Ip= (Vp1 - Ve1) / Constants.LMK_RL[rootPage.settingData[i].CONFIG_STRUCT.voltageLevel] * Constants.LMK_V_KAD;
                            Ie= Ve1 / Constants.LMK_RL[rootPage.settingData[i].CONFIG_STRUCT.voltageLevel] * Constants.LMK_V_KAD;
                            In = Ip - Ie;
                            Vp = Ve + Ip * Constants.LMK_RH[rootPage.settingData[i].CONFIG_STRUCT.voltageLevel];
                            Vn = Ve - In * Constants.LMK_RH[rootPage.settingData[i].CONFIG_STRUCT.voltageLevel];
                            if (Ve > 0)
                            {
                                Rfn = -Vn / Ie ;
                                Rfp = Constants.MIN_R_FAULT;
                            }
                            else
                            {
                                Rfp = -Vp / Ie ;
                                Rfn = Constants.MIN_R_FAULT;
                            }
                            tempoLeakageData.Vp = Vp+ VOffset;
                            tempoLeakageData.Vn = Vn- + VOffset;
                            tempoLeakageData.Rfp = Rfp;
                            tempoLeakageData.Rfn= Rfn;
                            tempoLeakageData.If1 = (I11 - Constants.ELEVEN_POW) * Constants.LMK_I_KAD + I1Offset;
                            tempoLeakageData.If2 = (I12 - Constants.ELEVEN_POW) * Constants.LMK_I_KAD + I2Offset;


                            if ((T > Constants.MIN_HUMIDITY_VALUE) && (T < Constants.MAX_HUMIDITY_VALUE))
                            {
                                ushort integralPart = (ushort)(T / 256);
                                ushort decimalPart = (ushort)(T % 256);
                                tempoLeakageData.H = (float)integralPart + (float)(decimalPart / 100);
                            }
                            else tempoLeakageData.H = T = Constants.MIN_HUMIDITY_VALUE;
                            tempoLeakageData.timeTicks = DateTime.Now.Ticks;
                            leakageResult.Add(tempoLeakageData);

                        }
                        else if (setting[i].CONFIG_STRUCT.deviceType == Constants.SMK_TYPE)
                        {
                            V00 = (float)measurement[i].V0;
                            I01 = (float)measurement[i].V1;
                            I02 = (float)measurement[i].V2;
                            T = (float)measurement[i].T;

                            V10 = (float)measurement[i].sV1;
                            I11 = (float)measurement[i].sV2;
                            I12 = (float)measurement[i].E;

                            byte voltageLevel, currentLevel;
                            voltageLevel = setting[i].CONFIG_STRUCT.voltageLevel;
                            currentLevel = setting[i].CONFIG_STRUCT.blkCapacity;
                            float VOffset = (float)(rootPage.settingData[i].VSCALE - Constants.FITTHTEEN_POW) / 1000;
                            float I1Offset = (float)(rootPage.settingData[i].R1 - Constants.FITTHTEEN_POW) / 1000;
                            float I2Offset = (float)(rootPage.settingData[i].R2 - Constants.FITTHTEEN_POW) / 1000;

                            if ((voltageLevel < Constants.SMK_VOLTAGE_LEVEL_SIZE) && (currentLevel < Constants.SMK_CURRENT_LEVEL_SIZE))
                            {
                                result[i].V0 = Constants.A_VOLTAGE_COEF_AD[voltageLevel] - Constants.B_VOLTAGE_COEF_AD[voltageLevel] * V00 + VOffset;
                                result[i].V1 = Constants.A_CURRENT_COEF_AD[currentLevel] - Constants.B_CURRENT_COEF_AD[currentLevel] * I01 + I1Offset;
                                result[i].V2 = Constants.A_CURRENT_COEF_AD[currentLevel] - Constants.B_CURRENT_COEF_AD[currentLevel] * I02 + I2Offset;

                                result[i].E = Constants.A_VOLTAGE_COEF_AD[voltageLevel] - Constants.B_VOLTAGE_COEF_AD[voltageLevel] * V10 + VOffset;
                                result[i].R = Constants.A_CURRENT_COEF_AD[currentLevel] - Constants.B_CURRENT_COEF_AD[currentLevel] * I11 + I1Offset;
                                result[i].SOC = Constants.A_CURRENT_COEF_AD[currentLevel] - Constants.B_CURRENT_COEF_AD[currentLevel] * I12 + I2Offset;
                                if ((T > Constants.MIN_HUMIDITY_VALUE) && (T < Constants.MAX_HUMIDITY_VALUE))
                                {
                                    ushort integralPart = (ushort)(T / 256);
                                    ushort decimalPart = (ushort)(T % 256);
                                    result[i].T = (float)integralPart + (float)(decimalPart / 100);
                                }
                                else result[i].T = T = Constants.MIN_HUMIDITY_VALUE;
                            }
                            result[i].timeTicks = DateTime.Now.Ticks;
                        }
                    }
                    if (measurement[i].STATUS_STRUCT.alarm == Constants.V_DATA)
                    {
                        measurementLastV[i].Retrieve(measurementV[i]);
                        measurementV[i].Retrieve(tempo);
                        List<VarianceDataType> varianceKitData = new List<VarianceDataType>();
                        varianceData[i].Clear();
                        if (varianceDetect(measurementV[i], measurementLastV[i], ref varianceKitData, setting[i].CONFIG_STRUCT.voltageLevel))
                        {
                            // Build up an new records with format of measurementData
                            if (varianceKitData.Count != 0)
                            {
                                foreach (VarianceDataType variance in varianceKitData)
                                {
                                    varianceData[i].Add(variance);
                                }
                                rootPage.recordMustBeSavedDueV = true;
                            }
                        }

                    }
                }
                byteIndex += Constants.RECORDWIDTH;
            }
        }
        public void SettingDataRetrieve(ushort currentPage, ref List<SettingDataType> setting, byte[] raw)
        {
            byte[] tempo = new byte[Constants.RECORDWIDTH];
            int index = currentPage * rootPage.nbKitperPage;
            int byteIndex = Constants.DATARXOFFSET;
            int lenght;
            if (index + rootPage.nbKitperPage < rootPage.totalKit + 1) lenght = rootPage.nbKitperPage;                                  // Threre is nbKit+1 record being transferred including system records
            else lenght = rootPage.totalKit + 1 - index;
            for (int i = index; i < index + lenght; i++)
            {
                Array.Copy(raw, byteIndex, tempo, 0, Constants.RECORDWIDTH);
                setting[i].Retrieve(tempo);
                byteIndex += Constants.RECORDWIDTH;
            }
        }
        public void CalibrationDataRetrieve(ushort currentPage, ref List<SettingDataType> calibration, byte[] raw)
        {
            byte[] tempo = new byte[Constants.RECORDWIDTH];
            int index = currentPage * rootPage.nbKitperPage;
            int byteIndex = Constants.DATARXOFFSET;
            int lenght;
            if (index + rootPage.nbKitperPage < rootPage.totalKit + 1) lenght = rootPage.nbKitperPage;                                  // Threre is nbKit+1 record being transferred including system records
            else lenght = rootPage.totalKit + 1 - index;
            for (int i = index; i < index + lenght; i++)
            {
                Array.Copy(raw, byteIndex, tempo, 0, Constants.RECORDWIDTH);
                calibration[i].Retrieve(tempo);
                byteIndex += Constants.RECORDWIDTH;
            }
        }
        string EventMessageCompose(String message, List<int> IdList, long timeTicks, string msgHeader)
        {
            String content = message + System.Environment.NewLine;
            if (content.Length > Constants.MAX_CHAR_ALARM) content = "";
            DateTime tempoDateTime = new DateTime(timeTicks);
            content += tempoDateTime.ToLongTimeString();
            content += " : " + msgHeader;
            int unit, str, blk;
            unit = str = blk = 0;
            Dictionary<int, List<int>> alarmList = new Dictionary<int, List<int>>();
            List<int> blkList;
            foreach (int i in IdList)
            {
                int type= Constants.BMK_TYPE;
                rootPage.GetId(rootPage.settingData, i, ref unit, ref str, ref blk , ref type);
                int strId = rootPage.GetRecord(rootPage.settingData, unit, str , blk , Constants.SMK_TYPE);
                if (strId != i)                     // It's not SMK
                {
                    if (alarmList.TryGetValue(strId, out blkList))
                    {
                        blkList.Add(blk);
                    }
                    else
                    {
                        List<int> tempoList = new List<int>();
                        tempoList.Add(blk);
                        alarmList.Add(strId, tempoList);
                    }
                }
            }

            foreach (var alarmItem in alarmList)
            {
                int strId = rootPage.settingData[alarmItem.Key].STRING;
                int unitId = rootPage.settingData[alarmItem.Key].UNIT;
                content += " : Unit " + (unitId + 1).ToString("N0") + " String " + (strId + 1).ToString("N0") + " Block ";
                foreach (int blkId in alarmItem.Value)
                    if (blkId != alarmItem.Value.Last())
                        content += (blkId + 1).ToString() + "/";
                    else content += (blkId + 1).ToString() + ".";
            }
            return content;
        }
        public void AlarmCheck(MeasurementDataPackage pmDataPackage, List<MeasurementDataType> pmeasurementList, List<SettingDataType> pSettingDataList)
        {
            int unit, str, blk;
            unit = str = blk = 0;
            string eventMessage = "";
            List<int> R_Upper = new List<int>();
            List<int> V_Upper = new List<int>();
            List<int> V_Lower = new List<int>();
            List<int> E_Upper = new List<int>();
            List<int> E_Lower = new List<int>();
            List<int> T_Upper = new List<int>();
            List<int> T_Lower = new List<int>();
            int type = 0;
            for (int i = 1; i < pmDataPackage.displayData.Count(); i++)
            {
                rootPage.GetId(rootPage.settingData, i, ref unit, ref str, ref blk , ref type);
                if(type==Constants.BMK_TYPE)
                {
                    if (blk == 0) rootPage.nbAlarmBlk[unit] = 0;                       // Summerizing the total alarm block in each unit 
                    if (pmeasurementList[i].STATUS_STRUCT.presented)
                    {
                        if (rootPage.settingData[i].CONFIG_STRUCT.deviceType == Constants.BMK_TYPE)
                        {
                            if (pmDataPackage.displayData[i].R > rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MAX, (int)DATA_INDEX.R, rootPage.alarmSettingData.R_Upper])
                            {
                                rootPage.alarmData[i].R_Upper = true;
                                rootPage.alarmData[i].NB_R_Upper++;
                                R_Upper.Add(i);
                            }
                            else rootPage.alarmData[i].R_Upper = false;
                            if (pmDataPackage.displayData[i].V0 > rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MAX, (int)DATA_INDEX.V, rootPage.alarmSettingData.V_Upper])
                            {
                                rootPage.alarmData[i].V_Upper = true;
                                rootPage.alarmData[i].NB_V_Upper++;
                                V_Upper.Add(i);
                            }
                            else rootPage.alarmData[i].V_Upper = false;
                            if (pmDataPackage.displayData[i].V0 < rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MIN, (int)DATA_INDEX.V, rootPage.alarmSettingData.V_Lower])
                            {
                                rootPage.alarmData[i].V_Lower = true;
                                rootPage.alarmData[i].NB_V_Lower++;
                                V_Lower.Add(i);
                            }
                            else rootPage.alarmData[i].V_Lower = false;
                            if (pmDataPackage.displayData[i].E > rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MAX, (int)DATA_INDEX.E, rootPage.alarmSettingData.E_Upper])
                            {
                                rootPage.alarmData[i].E_Upper = true;
                                rootPage.alarmData[i].NB_E_Upper++;
                                E_Upper.Add(i);
                            }
                            else rootPage.alarmData[i].E_Upper = false;
                            if (pmDataPackage.displayData[i].E < rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MIN, (int)DATA_INDEX.E, rootPage.alarmSettingData.E_Lower])
                            {
                                rootPage.alarmData[i].E_Lower = true;
                                rootPage.alarmData[i].NB_E_Lower++;
                                E_Lower.Add(i);
                            }
                            else rootPage.alarmData[i].E_Lower = false;
                            if (pmDataPackage.displayData[i].T > rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MAX, (int)DATA_INDEX.T, rootPage.alarmSettingData.T_Upper])
                            {
                                rootPage.alarmData[i].T_Upper = true;
                                rootPage.alarmData[i].NB_T_Upper++;
                                T_Upper.Add(i);
                            }
                            else rootPage.alarmData[i].T_Upper = false;
                            if (pmDataPackage.displayData[i].T < rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MIN, (int)DATA_INDEX.T, rootPage.alarmSettingData.T_Lower])
                            {
                                rootPage.alarmData[i].T_Lower = true;
                                rootPage.alarmData[i].NB_T_Lower++;
                                T_Lower.Add(i);
                            }
                            else rootPage.alarmData[i].T_Lower = false;
                            if (rootPage.alarmData[i].R_Upper || rootPage.alarmData[i].V_Upper || rootPage.alarmData[i].E_Lower || rootPage.alarmData[i].T_Upper) rootPage.nbAlarmBlk[unit]++;
                        }
                    }
                }
                else
                {
                    rootPage.alarmData[i].NB_Missed++;
                }
            }
            if (R_Upper.Count() != 0)
                eventMessage = EventMessageCompose(eventMessage, R_Upper, pmDataPackage.timeTicks, "Resistance is over threshold of " + rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MAX, (int)DATA_INDEX.R, rootPage.alarmSettingData.R_Upper].ToString("N3") + " mΩ");
            if (V_Upper.Count() != 0)
                eventMessage = EventMessageCompose(eventMessage, V_Upper, pmDataPackage.timeTicks, "Floating voltage is over threshold of " + rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MAX, (int)DATA_INDEX.V, rootPage.alarmSettingData.V_Upper].ToString("N3") + " V");
            if (V_Lower.Count() != 0)
                eventMessage = EventMessageCompose(eventMessage, V_Lower, pmDataPackage.timeTicks, "Floating voltage is lower than threshold of " + rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MIN, (int)DATA_INDEX.V, rootPage.alarmSettingData.V_Lower].ToString("N3") + " V");
            if (E_Upper.Count() != 0)
                eventMessage = EventMessageCompose(eventMessage, E_Upper, pmDataPackage.timeTicks, "Open voltage is over threshold of " + rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MAX, (int)DATA_INDEX.E, rootPage.alarmSettingData.E_Upper].ToString("N3") + " V");
            if (E_Lower.Count() != 0)
                eventMessage = EventMessageCompose(eventMessage, E_Lower, pmDataPackage.timeTicks, "Open voltage is lower than threshold of " + rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MIN, (int)DATA_INDEX.E, rootPage.alarmSettingData.E_Lower].ToString("N3") + " V");
            if (T_Upper.Count() != 0)
                eventMessage = EventMessageCompose(eventMessage, T_Upper, pmDataPackage.timeTicks, "Temperature is over threshold of " + rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MAX, (int)DATA_INDEX.T, rootPage.alarmSettingData.T_Upper].ToString("N1") + " ºC");
            if (T_Lower.Count() != 0)
                eventMessage = EventMessageCompose(eventMessage, T_Lower, pmDataPackage.timeTicks, "Temperature is lower than threshold of " + rootPage.thresholdAlarm[(int)unit, (int)RANGE_INDEX.MIN, (int)DATA_INDEX.T, rootPage.alarmSettingData.T_Lower].ToString("N1") + " ºC");
            rootPage.eventMessage = String.Copy(eventMessage);
        }
        public bool ParseData(List<MeasurementDataPackage> DataPackageList)
        {
            List<DataPoint> GList = new List<DataPoint>();
            List<string> TimeList = new List<string>();
            List<float> VList = new List<float>();
            //            DateTime dateTime = new DateTime();
            foreach (MeasurementDataPackage dataPack in DataPackageList)
            {
                foreach (var item in dataPack.displayData)
                {
                    DateTime dateTime = new DateTime(item.timeTicks);
                    TimeList.Add(dateTime.ToString("HH:mm:ss"));
                    VList.Add(item.V0);
                }
            }
            return true;
        }
    }
}
