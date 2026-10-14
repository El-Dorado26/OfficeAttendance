
using System;
using System.Linq;
using OfficeAttendance.Data;
using OfficeAttendance.Models;
using ClosedXML.Excel;

namespace OfficeAttendance.Services
{
    public class AttendanceExportService
    {
        public void ExportEmployeeAttendance(
            string username,
            string filePath)
        {
            using (var db = new AppDbContext())
            {
                var attendanceRecords = db.Attendances
                    .Where(a => a.Username == username)
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.CheckIn)
                    .ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet =
                        workbook.Worksheets.Add("Attendance");

                    string[] headers =
                    {
                        "Date",
                        "Check In",
                        "Check Out",
                        "Break Duration",
                        "Net Working Hours",
                        "Session Status"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                    }

                    var headerRange = worksheet.Range(1, 1, 1, 6);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor =
                        XLColor.LightBlue;

                    int row = 2;

                    foreach (Attendance attendance in attendanceRecords)
                    {
                        var breaks = db.AttendanceBreaks
                            .Where(b => b.AttendanceId == attendance.ID)
                            .ToList();

                        TimeSpan totalBreak = TimeSpan.Zero;

                        foreach (AttendanceBreak attendanceBreak in breaks)
                        {
                            if (attendanceBreak.BreakEnd != null)
                            {
                                totalBreak +=
                                    attendanceBreak.BreakEnd.Value -
                                    attendanceBreak.BreakStart;
                            }
                        }

                        worksheet.Cell(row, 1).Value = attendance.Date;
                        worksheet.Cell(row, 1).Style.DateFormat.Format =
                            "yyyy-mm-dd";

                        if (attendance.CheckIn != null)
                        {
                            worksheet.Cell(row, 2).Value =
                                attendance.CheckIn.Value;
                            worksheet.Cell(row, 2).Style.DateFormat.Format =
                                "hh:mm AM/PM";
                        }

                        if (attendance.CheckOut != null)
                        {
                            worksheet.Cell(row, 3).Value =
                                attendance.CheckOut.Value;
                            worksheet.Cell(row, 3).Style.DateFormat.Format =
                                "hh:mm AM/PM";
                        }

                        worksheet.Cell(row, 4).Value =
                            FormatDuration(totalBreak);

                        if (attendance.CheckIn != null &&
                            attendance.CheckOut != null)
                        {
                            TimeSpan workingTime =
                                attendance.CheckOut.Value -
                                attendance.CheckIn.Value -
                                totalBreak;

                            worksheet.Cell(row, 5).Value =
                                FormatDuration(workingTime);

                            worksheet.Cell(row, 6).Value = "Completed";
                        }
                        else if (attendance.CheckIn != null)
                        {
                            worksheet.Cell(row, 5).Value = "In progress";
                            worksheet.Cell(row, 6).Value = "Working";
                        }
                        else
                        {
                            worksheet.Cell(row, 5).Value = "-";
                            worksheet.Cell(row, 6).Value = "Not checked in";
                        }

                        row++;
                    }

                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(filePath);
                }
            }
        }

        private string FormatDuration(TimeSpan duration)
        {
            return $"{(int)duration.TotalHours}h {duration.Minutes}m";
        }
    }
}
