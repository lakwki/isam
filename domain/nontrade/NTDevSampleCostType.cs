using System;
using System.Collections;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class NTDevSampleCostType : DomainData 
    {
        public static NTDevSampleCostType SAMPLE = new NTDevSampleCostType(Code.SML);
        public static NTDevSampleCostType PRINTING = new NTDevSampleCostType(Code.PRT);
        public static NTDevSampleCostType DYE = new NTDevSampleCostType(Code.DYE);
        public static NTDevSampleCostType GBTEST = new NTDevSampleCostType(Code.GBTEST);
        public static NTDevSampleCostType SMLROOM = new NTDevSampleCostType(Code.SMLROOM);
        public static NTDevSampleCostType MATFAB = new NTDevSampleCostType(Code.MATFAB);
        public static NTDevSampleCostType MATTRIM = new NTDevSampleCostType(Code.MATTRIM);
        public static NTDevSampleCostType TESTBASE = new NTDevSampleCostType(Code.TESTBASE);
        public static NTDevSampleCostType COU = new NTDevSampleCostType(Code.COU);
        public static NTDevSampleCostType FABDEV = new NTDevSampleCostType(Code.FAB_DEV);
        public static NTDevSampleCostType AIR = new NTDevSampleCostType(Code.AIR);
        public static NTDevSampleCostType MODEL = new NTDevSampleCostType(Code.MODEL);

        private Code _code;

        private NTDevSampleCostType(Code code)
        {
            this._code = code;
        }

        private enum Code
        {
            SML = 1,
            PRT = 2,
            DYE = 3,
            GBTEST = 4,
            SMLROOM = 5,
            MATFAB = 6,
            MATTRIM = 7,
            TESTBASE = 8,
            COU = 9,
            FAB_DEV = 10,
            AIR = 11,
            MODEL = 12
        }

        public int Id
        {
            get
            {
                return Convert.ToUInt16(_code.GetHashCode());
            }
        }

        public string Description
        {
            get
            {
                switch (_code)
                {
                    case Code.SML :
                        return "Sample (Finished Product)";
                    case Code.PRT :
                        return "Printing";
                    case Code.DYE :
                        return "Dye";
                    case Code.GBTEST:
                        return "GB Testing Charge";
                    case Code.SMLROOM:
                        return "Sample Room Charge (Pattern and Sewing)";
                    case Code.MATFAB:
                        return "Material - Fabric";
                    case Code.MATTRIM:
                        return "Material - Trims";
                    case Code.TESTBASE:
                        return "Lab Test (Base)";
                    case Code.COU:
                        return "Courier";
                    case Code.FAB_DEV:
                        return "Fabric Development";
                    case Code.AIR:
                        return "Air Freight";
                    case Code.MODEL:
                        return "Model Fee";
                    default:
                        return "UNDEFINED";
                }
            }
        }

        public string SampleCostTypeCode
        {
            get
            {
                switch (_code)
                {
                    case Code.SML:
                        return "SML";
                    case Code.PRT:
                        return "PRT";
                    case Code.DYE:
                        return "DYE";
                    case Code.GBTEST:
                        return "GBTEST";
                    case Code.SMLROOM:
                        return "SMLROOM";
                    case Code.MATFAB:
                        return "MATFAB";
                    case Code.MATTRIM:
                        return "MATTRIM";
                    case Code.TESTBASE:
                        return "TESTBASE";
                    case Code.COU:
                        return "COU";
                    case Code.FAB_DEV:
                        return "FABDEV";
                    case Code.AIR:
                        return "AIR";
                    case Code.MODEL:
                        return "MODEL";
                    default:
                        return "UNDEFINED";
                }
            }
        }

        public static NTDevSampleCostType getType(int id)
        {
            if (id == Code.SML.GetHashCode()) return NTDevSampleCostType.SAMPLE;
            else if (id == Code.PRT.GetHashCode()) return NTDevSampleCostType.PRINTING;
            else if (id == Code.DYE.GetHashCode()) return NTDevSampleCostType.DYE;
            else if (id == Code.GBTEST.GetHashCode()) return NTDevSampleCostType.GBTEST;
            else if (id == Code.SMLROOM.GetHashCode()) return NTDevSampleCostType.SMLROOM;
            else if (id == Code.MATFAB.GetHashCode()) return NTDevSampleCostType.MATFAB;
            else if (id == Code.MATTRIM.GetHashCode()) return NTDevSampleCostType.MATTRIM;
            else if (id == Code.TESTBASE.GetHashCode()) return NTDevSampleCostType.TESTBASE;
            else if (id == Code.COU.GetHashCode()) return NTDevSampleCostType.COU;
            else if (id == Code.FAB_DEV.GetHashCode()) return NTDevSampleCostType.FABDEV;
            else if (id == Code.AIR.GetHashCode()) return NTDevSampleCostType.AIR;
            else if (id == Code.MODEL.GetHashCode()) return NTDevSampleCostType.MODEL;
            else return null;
        }

        public static ArrayList getNTDevSampleCostTypeList()
        {
            ArrayList list = new ArrayList();
            list.Add(NTDevSampleCostType.SAMPLE);
            list.Add(NTDevSampleCostType.PRINTING);
            list.Add(NTDevSampleCostType.DYE);
            list.Add(NTDevSampleCostType.GBTEST);
            list.Add(NTDevSampleCostType.SMLROOM);
            list.Add(NTDevSampleCostType.MATFAB);
            list.Add(NTDevSampleCostType.MATTRIM);
            list.Add(NTDevSampleCostType.TESTBASE);
            list.Add(NTDevSampleCostType.COU);
            list.Add(NTDevSampleCostType.FABDEV);
            list.Add(NTDevSampleCostType.AIR);
            list.Add(NTDevSampleCostType.MODEL);
            return list;
        }
    }
}
