using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GuildConnect_Desktop.Services
{
    /// <summary>
    /// Utility class for generating CSV reports from DataGridView or DataTable data.
    /// Used by both Admin and Society Head dashboards.
    /// </summary>
    public static class ReportService
    {
        /// <summary>
        /// Exports a DataGridView's visible data to a CSV file.
        /// Opens a SaveFileDialog for the user to choose the location.
        /// </summary>
        public static void ExportToCsv(DataGridView dgv, string defaultFileName)
        {
            if (dgv == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt";
                sfd.FileName = defaultFileName;
                sfd.Title = "Export Report";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sb = new StringBuilder();

                        // Header row - only visible columns
                        var headers = new StringBuilder();
                        foreach (DataGridViewColumn col in dgv.Columns)
                        {
                            if (col.Visible)
                            {
                                if (headers.Length > 0) headers.Append(",");
                                headers.Append(EscapeCsv(col.HeaderText));
                            }
                        }
                        sb.AppendLine(headers.ToString());

                        // Data rows
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            if (row.IsNewRow) continue;
                            var line = new StringBuilder();
                            foreach (DataGridViewColumn col in dgv.Columns)
                            {
                                if (col.Visible)
                                {
                                    if (line.Length > 0) line.Append(",");
                                    var cellValue = row.Cells[col.Index].Value?.ToString() ?? "";
                                    line.Append(EscapeCsv(cellValue));
                                }
                            }
                            sb.AppendLine(line.ToString());
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show($"Report exported successfully!\n\n{sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Export failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Exports multiple DataGridViews as sections in a single combined report.
        /// </summary>
        public static void ExportCombinedReport(string defaultFileName, params (string SectionTitle, DataGridView Grid)[] sections)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt";
                sfd.FileName = defaultFileName;
                sfd.Title = "Export Combined Report";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine($"GuildConnect Report — Generated {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        sb.AppendLine(new string('=', 60));
                        sb.AppendLine();

                        foreach (var section in sections)
                        {
                            sb.AppendLine($"--- {section.SectionTitle} ---");

                            if (section.Grid == null || section.Grid.Rows.Count == 0)
                            {
                                sb.AppendLine("(No data)");
                                sb.AppendLine();
                                continue;
                            }

                            // Header
                            var headers = new StringBuilder();
                            foreach (DataGridViewColumn col in section.Grid.Columns)
                            {
                                if (col.Visible)
                                {
                                    if (headers.Length > 0) headers.Append(",");
                                    headers.Append(EscapeCsv(col.HeaderText));
                                }
                            }
                            sb.AppendLine(headers.ToString());

                            // Data
                            foreach (DataGridViewRow row in section.Grid.Rows)
                            {
                                if (row.IsNewRow) continue;
                                var line = new StringBuilder();
                                foreach (DataGridViewColumn col in section.Grid.Columns)
                                {
                                    if (col.Visible)
                                    {
                                        if (line.Length > 0) line.Append(",");
                                        var cellValue = row.Cells[col.Index].Value?.ToString() ?? "";
                                        line.Append(EscapeCsv(cellValue));
                                    }
                                }
                                sb.AppendLine(line.ToString());
                            }
                            sb.AppendLine();
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show($"Combined report exported successfully!\n\n{sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Export failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Properly escapes a value for CSV output.
        /// </summary>
        private static string EscapeCsv(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}
