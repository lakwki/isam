using System;
using System.Collections;
using com.next.common.domain;


namespace com.next.isam.domain.types
{
    [Serializable()]
    public class ExpenseNature : DomainData 
    {

        public static ExpenseNature BONUS = new ExpenseNature(Code.Bonus);
        public static ExpenseNature DOUBLE_PAY = new ExpenseNature(Code.DoublePay);
        public static ExpenseNature HOUSING_ALLOWANCE = new ExpenseNature(Code.HousingAllowance);
        public static ExpenseNature INSURANCE = new ExpenseNature(Code.Insurance);
        public static ExpenseNature MISCELLANEOUS = new ExpenseNature(Code.Miscellaneous);
        public static ExpenseNature MOBILE = new ExpenseNature(Code.Mobile);
        public static ExpenseNature OTHERS = new ExpenseNature(Code.Others);
        public static ExpenseNature OFFICE_RENT = new ExpenseNature(Code.OfficeRent);
        public static ExpenseNature SALARY = new ExpenseNature(Code.Salary);
        public static ExpenseNature SOCIAL_SECURITY = new ExpenseNature(Code.SocialSecurity);
        public static ExpenseNature TRAVEL = new ExpenseNature(Code.Travel);
        public static ExpenseNature LOCAL_TRAVEL = new ExpenseNature(Code.LocalTravel);
        public static ExpenseNature COURIER = new ExpenseNature(Code.Courier);
        public static ExpenseNature UTILITIES = new ExpenseNature(Code.Utilities);
        public static ExpenseNature TESTING_SERVICE = new ExpenseNature(Code.TestingService);
        public static ExpenseNature MOTOR_VEHICLE = new ExpenseNature(Code.MotorVehicle);
        public static ExpenseNature SAMPLE_FABRIC = new ExpenseNature(Code.SampleFabric);
        public static ExpenseNature ENTERTAINMENT = new ExpenseNature(Code.Entertainment);
        public static ExpenseNature TELECOM = new ExpenseNature(Code.Telecom);
        public static ExpenseNature PROFESSIONAL_FEE = new ExpenseNature(Code.ProfessionalFee);
        public static ExpenseNature IT_EXPENSE = new ExpenseNature(Code.ITExpense);
        public static ExpenseNature RECRUITMENT = new ExpenseNature(Code.Recruitment);
        public static ExpenseNature AIR_FREIGHT = new ExpenseNature(Code.AirFreight);
        public static ExpenseNature PRINTING = new ExpenseNature(Code.Printing);



        private Code _code;

        private enum Code
        {
            Bonus = 1,
            DoublePay = 2,
            HousingAllowance = 3,
            Insurance = 4,
            Miscellaneous = 5,
            Mobile = 6,
            OfficeRent = 7,
            Salary = 8,
            SocialSecurity = 9,
            Travel = 10,
            Others = 11,
            LocalTravel = 12,
            Courier = 13,
            Utilities = 14,
            TestingService = 15,
            MotorVehicle = 16,
            SampleFabric = 17,
            Entertainment = 18,
            Telecom = 19,
            ProfessionalFee = 20,
            ITExpense = 21,
            Recruitment = 22,
            AirFreight = 23,
            Printing = 24
        }

        private ExpenseNature(Code code)
        {
            this._code = code;
        }

        public int Id
        {
            get
            {
                return Convert.ToUInt16(_code.GetHashCode());
            }
        }

        public string AnalysisCode
        {
            get
            {
                switch (_code)
                {
                    case Code.Bonus :
                        return "BON";                    
                    case Code.DoublePay :
                        return "DBP";
                    case Code.HousingAllowance:
                        return "HOU";
                    case Code.Insurance :
                        return "INS";
                    case Code.Miscellaneous:
                        return "MIS";
                    case Code.Mobile:
                        return "MOB";
                    case Code.Others:
                        return "OTA";
                    case Code.OfficeRent:
                        return "REN";
                    case Code.Salary:
                        return "SAL";
                    case Code.SocialSecurity:
                        return "SOC";
                    case Code.Travel:
                        return "TRA";
                    case Code.LocalTravel:
                        return "LOC";
                    case Code.Courier:
                        return "COU";
                    case Code.Utilities:
                        return "UTI";
                    case Code.TestingService:
                        return "TST";
                    case Code.MotorVehicle:
                        return "TST";
                    case Code.SampleFabric:
                        return "FAB";
                    case Code.Entertainment:
                        return "ENT";
                    case Code.Telecom:
                        return "TEL";
                    case Code.ProfessionalFee:
                        return "PRO";
                    case Code.ITExpense:
                        return "ITE";
                    case Code.Recruitment:
                        return "REC";
                    case Code.AirFreight:
                        return "AIR";
                    case Code.Printing:
                        return "PRI";
                    default:
                        return "UNDEFINED";
                }
            }
        }


        public static ExpenseNature getType(int id)
        {
            if (id == Code.Bonus.GetHashCode()) return ExpenseNature.BONUS;
            else if (id == Code.DoublePay.GetHashCode()) return ExpenseNature.DOUBLE_PAY;
            else if (id == Code.HousingAllowance.GetHashCode()) return ExpenseNature.HOUSING_ALLOWANCE;
            else if (id == Code.Insurance.GetHashCode()) return ExpenseNature.INSURANCE;
            else if (id == Code.Miscellaneous.GetHashCode()) return ExpenseNature.MISCELLANEOUS;
            else if (id == Code.Mobile.GetHashCode()) return ExpenseNature.MOBILE;
            else if (id == Code.Others.GetHashCode()) return ExpenseNature.OTHERS;
            else if (id == Code.OfficeRent.GetHashCode()) return ExpenseNature.OFFICE_RENT;
            else if (id == Code.Salary.GetHashCode()) return ExpenseNature.OFFICE_RENT;
            else if (id == Code.SocialSecurity.GetHashCode()) return ExpenseNature.SOCIAL_SECURITY;
            else if (id == Code.Travel.GetHashCode()) return ExpenseNature.TRAVEL;
            else if (id == Code.LocalTravel.GetHashCode()) return ExpenseNature.LOCAL_TRAVEL;
            else if (id == Code.Courier.GetHashCode()) return ExpenseNature.COURIER;
            else if (id == Code.Utilities.GetHashCode()) return ExpenseNature.UTILITIES;
            else if (id == Code.TestingService.GetHashCode()) return ExpenseNature.TESTING_SERVICE;
            else if (id == Code.MotorVehicle.GetHashCode()) return ExpenseNature.MOTOR_VEHICLE;
            else if (id == Code.SampleFabric.GetHashCode()) return ExpenseNature.SAMPLE_FABRIC;
            else if (id == Code.Entertainment.GetHashCode()) return ExpenseNature.ENTERTAINMENT;
            else if (id == Code.Telecom.GetHashCode()) return ExpenseNature.TELECOM;
            else if (id == Code.ProfessionalFee.GetHashCode()) return ExpenseNature.PROFESSIONAL_FEE;
            else if (id == Code.ITExpense.GetHashCode()) return ExpenseNature.IT_EXPENSE;
            else if (id == Code.Recruitment.GetHashCode()) return ExpenseNature.RECRUITMENT;
            else if (id == Code.AirFreight.GetHashCode()) return ExpenseNature.AIR_FREIGHT;
            else if (id == Code.Printing.GetHashCode()) return ExpenseNature.PRINTING;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(ExpenseNature.BONUS);
            list.Add(ExpenseNature.DOUBLE_PAY);
            list.Add(ExpenseNature.HOUSING_ALLOWANCE);
            list.Add(ExpenseNature.INSURANCE);
            list.Add(ExpenseNature.MISCELLANEOUS);
            list.Add(ExpenseNature.MOBILE);
            list.Add(ExpenseNature.OFFICE_RENT);
            list.Add(ExpenseNature.OTHERS);
            list.Add(ExpenseNature.SALARY);
            list.Add(ExpenseNature.SOCIAL_SECURITY);
            list.Add(ExpenseNature.TRAVEL);
            list.Add(ExpenseNature.LOCAL_TRAVEL);
            list.Add(ExpenseNature.COURIER);
            list.Add(ExpenseNature.UTILITIES);
            list.Add(ExpenseNature.TESTING_SERVICE);
            list.Add(ExpenseNature.MOTOR_VEHICLE);
            list.Add(ExpenseNature.SAMPLE_FABRIC);
            list.Add(ExpenseNature.ENTERTAINMENT);
            list.Add(ExpenseNature.TELECOM);
            list.Add(ExpenseNature.PROFESSIONAL_FEE);
            list.Add(ExpenseNature.IT_EXPENSE);
            list.Add(ExpenseNature.RECRUITMENT);
            list.Add(ExpenseNature.AIR_FREIGHT);
            list.Add(ExpenseNature.PRINTING);
            return list;
        }
    }
}
