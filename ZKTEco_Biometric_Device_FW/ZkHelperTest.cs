using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using zkemkeeper;

namespace ZKTEco_Biometric_Device_FW
{
    public class ZkHelperTest
    {


        //int idwErrorCode = 0;
        CultureInfo gregorianCulture = new CultureInfo("en-US");
        private string IFaceBrnachName = "G1";
        private string IFaceBrnachCode = "00215";
        private const string version = "3.0";
    
        public List<TransactionDto> GetAttendanceLogs(string iface_Ip, int port, int commKey, DateTime startDate , DateTime endDate)
        {
            List<TransactionDto> logs = new List<TransactionDto>();
            try
            {
                // Helper.Log("Connecting to " + iface_Ip + " ...");
                CZKEM objCZKEM = new CZKEM();
                if (commKey != 0)
                {
                    objCZKEM.SetCommPassword(commKey);
                }
                if (objCZKEM.Connect_Net(iface_Ip, port))
                {
                    Console.WriteLine("Connection Successful!");
                }
                string machineSerialNumber = string.Empty;
                objCZKEM.GetSerialNumber(objCZKEM.MachineNumber, out machineSerialNumber);

                //disable the device
                objCZKEM.EnableDevice(objCZKEM.MachineNumber, false);
                if (objCZKEM.ReadGeneralLogData(objCZKEM.MachineNumber))
                {
             
                    CultureInfo.CurrentCulture = gregorianCulture;
                    CultureInfo.CurrentUICulture = gregorianCulture;

                   // DateTime endDate = DateTime.Now;
                   

                    string dwEnrollNumber;
                    int dwVerifyMode;
                    int dwInOutMode;
                    int dwYear;
                    int dwMonth;
                    int dwDay;
                    int dwHour;
                    int dwMinute;
                    int dwSecond;
                    int dwWorkCode = 1;
                    int AWorkCode;

                    //Device user Info
                    string sdwEmployeeName = "";
                    string sdwPassword = "";
                    int sdwPrivilege = 0;
                    bool sdwEnabled = false;

                    objCZKEM.GetWorkCode(dwWorkCode, out AWorkCode);
                    int i = 0;

                    // Specify the format and culture for parsing
                    //string format = "yyyy-MM-dd HH:mm:ss";

                    while (objCZKEM.SSR_GetGeneralLogData(objCZKEM.MachineNumber, out dwEnrollNumber, out dwVerifyMode,
                                   out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))//get records from the memory
                    {


                        //2023-02-28 12:34:56.789
                        objCZKEM.SSR_GetUserInfo(objCZKEM.MachineNumber, dwEnrollNumber, out sdwEmployeeName, out sdwPassword, out sdwPrivilege, out sdwEnabled);

                        string dateString = $"{dwYear:D4}-{dwMonth:D2}-{dwDay:D2} {dwHour:D2}:{dwMinute:D2}:{dwSecond:D2}";

                        if (DateTime.TryParse(dateString, out DateTime m_tranactiondate))
                        {
                            if (m_tranactiondate >= startDate &&
                            m_tranactiondate <= endDate)
                            {
                                i++;
                                logs.Add(new TransactionDto()
                                {
                                    EmployeeCode = dwEnrollNumber,
                                    EmployeeName = sdwEmployeeName,
                                    BranchName = this.IFaceBrnachName,
                                    BranchCode = this.IFaceBrnachCode,
                                    MachinSerialNo = machineSerialNumber,
                                    TransactionDirection = dwInOutMode.ToString(),
                                    TransactionTime = m_tranactiondate,
                                    DateTimeRecord = dateString
                                });

                            }
                        }
                        else
                        {
                            _ = LogInfo($"Error v{version}: Connection Failed! {iface_Ip} - invalid transaction date (date now {DateTime.Now}) - date (string date {dateString}) ");
                            continue;
                        }
                    }
                }



            }
            catch (Exception ex)
            {

                _ = LogInfo($"Error v{version}:Connection Failed! {iface_Ip} - {ex.Message}");

            }
            return logs;

        }


        public List<TransactionDto> GetAttendanceLogs2(string iface_Ip, int port, int commKey, DateTime startDate, DateTime endDate)
        {
            List<TransactionDto> logs = new List<TransactionDto>();
            CZKEM objCZKEM = new CZKEM();

            try
            {
                // Set communication password if provided
                if (commKey != 0)
                {
                    objCZKEM.SetCommPassword(commKey);
                }

                // Connect to the device
                if (!objCZKEM.Connect_Net(iface_Ip, port))
                {
                    LogInfo($"Error v{version}: Unable to connect to device at {iface_Ip}:{port}");
                    return logs;
                }

                Console.WriteLine("Connection Successful!");

                // Get machine serial number
                if (!objCZKEM.GetSerialNumber(objCZKEM.MachineNumber, out string machineSerialNumber))
                {
                    LogInfo($"Error v{version}: Failed to retrieve serial number for {iface_Ip}");
                    return logs;
                }

                // Disable the device to read logs
                objCZKEM.EnableDevice(objCZKEM.MachineNumber, false);

                if (!objCZKEM.ReadGeneralLogData(objCZKEM.MachineNumber))
                {
                    LogInfo($"Error v{version}: Failed to read log data from device at {iface_Ip}");
                    return logs;
                }

                // Set culture for date handling
                CultureInfo.CurrentCulture = gregorianCulture;
                CultureInfo.CurrentUICulture = gregorianCulture;

                string dwEnrollNumber;
                int dwVerifyMode;
                int dwInOutMode;
                int dwYear;
                int dwMonth;
                int dwDay;
                int dwHour;
                int dwMinute;
                int dwSecond;
                int dwWorkCode;

                // Iterate through log data
                while (objCZKEM.SSR_GetGeneralLogData(objCZKEM.MachineNumber, out dwEnrollNumber, out dwVerifyMode,
                                                       out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour,
                                                       out dwMinute, out dwSecond, ref dwWorkCode))
                {
                    string dateString = $"{dwYear:D4}-{dwMonth:D2}-{dwDay:D2} {dwHour:D2}:{dwMinute:D2}:{dwSecond:D2}";

                    if (!DateTime.TryParse(dateString, out DateTime transactionDate))
                    {
                        LogInfo($"Error v{version}: Invalid transaction date (string date: {dateString}) at {iface_Ip}");
                        continue;
                    }

                    if (transactionDate >= startDate && transactionDate <= endDate)
                    {
                        if (objCZKEM.SSR_GetUserInfo(objCZKEM.MachineNumber, dwEnrollNumber, out string employeeName,
                                                      out _, out _, out _))
                        {
                            logs.Add(new TransactionDto()
                            {
                                EmployeeCode = dwEnrollNumber,
                                EmployeeName = employeeName,
                                BranchName = this.IFaceBrnachName,
                                BranchCode = this.IFaceBrnachCode,
                                MachinSerialNo = machineSerialNumber,
                                TransactionDirection = dwInOutMode.ToString(),
                                TransactionTime = transactionDate,
                                DateTimeRecord = dateString
                            });
                        }
                        else
                        {
                            LogInfo($"Error v{version}: Failed to retrieve user info for employee {dwEnrollNumber} at {iface_Ip}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfo($"Error v{version}: Exception while processing logs from {iface_Ip} - {ex.Message}");
            }
            finally
            {
                // Re-enable the device to resume normal operations
                objCZKEM.EnableDevice(objCZKEM.MachineNumber, true);
                objCZKEM.Disconnect();
            }

            return logs;
        }





        public bool DeleteAttendanceLogsByRange(string iface_Ip, int port, int commKey, DateTime startDate, DateTime endDate)
        {
            bool isDeleted = false;
            try
            {
                CZKEM objCZKEM = new CZKEM();

                if (commKey != 0)
                {
                    objCZKEM.SetCommPassword(commKey);
                }

                if (!objCZKEM.Connect_Net(iface_Ip, port))
                {
                    Console.WriteLine("Failed to connect to the device.");
                    return false;
                }

                Console.WriteLine("Connection successful.");

                objCZKEM.EnableDevice(objCZKEM.MachineNumber, false); // Disable the device

                if (objCZKEM.ReadGeneralLogData(objCZKEM.MachineNumber))
                {
                    string dwEnrollNumber;
                    int dwVerifyMode;
                    int dwInOutMode;
                    int dwYear;
                    int dwMonth;
                    int dwDay;
                    int dwHour;
                    int dwMinute;
                    int dwSecond;
                    int dwWorkCode = 1;

                    while (objCZKEM.SSR_GetGeneralLogData(objCZKEM.MachineNumber, out dwEnrollNumber, out dwVerifyMode,
                                   out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                    {
                        string dateString = $"{dwYear:D4}-{dwMonth:D2}-{dwDay:D2} {dwHour:D2}:{dwMinute:D2}:{dwSecond:D2}";

                        if (DateTime.TryParse(dateString, out DateTime logDate))
                        {
                            if (logDate >= startDate && logDate <= endDate)
                            {
                                // Delete log for the specific enroll number and date
                                if (!objCZKEM.SSR_DeleteEnrollData(objCZKEM.MachineNumber, dwEnrollNumber, 12)) // 12 refers to attendance logs
                                {
                                    Console.WriteLine($"Failed to delete log for user {dwEnrollNumber} on {dateString}.");
                                }
                                else
                                {
                                    isDeleted = true;
                                    Console.WriteLine($"Deleted log for user {dwEnrollNumber} on {dateString}.");
                                }
                            }
                        }
                    }
                }

                objCZKEM.EnableDevice(objCZKEM.MachineNumber, true); // Re-enable the device
                objCZKEM.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return isDeleted;
        }



        public async Task LogInfo(string message)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
               
            }
        }



    }
}
