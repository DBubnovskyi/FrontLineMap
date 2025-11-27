using MapDataProvider.DataSourceProviders.Contracts;
using MapDataProvider.Models;
using MapDataProvider.Models.MapElement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace DemoMap
{
    public static class FrontLineDataExporter
    {
        public static void SaveFrontLineFromDeepState(MapDataCollection result)
        {
            if (result.Metadata.Errors.Count > 0)
                return;

            string path = @"c:\frontline.dat";
            FileStream fs;
            try
            {
                fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            }
            catch (UnauthorizedAccessException)
            {
                if (!IsRunAsAdministrator())
                {
                    var dialogResult = MessageBox.Show(
                        "Для збереження файлу у C:\\ потрібні права адміністратора.\n\n" +
                        "Натисніть 'ТАК' щоб перезапустити програму з правами адміністратора\n" +
                        "Натисніть 'НІ' щоб зберегти файл у папку Documents",
                        "Потрібні права адміністратора",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        RestartAsAdministrator();
                        return;
                    }
                    else if (dialogResult == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = Path.Combine(documentsPath, "frontline.dat");

                try
                {
                    fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Неможливо створити файл: {ex.Message}", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            byte[] b = new byte[4];
            b[0] = 0xaa;
            b[1] = 0x46;
            b[2] = 0x4c;
            b[3] = 0xbb;
            fs.Write(b, 0, 4);

            int iCount;
            int iType;

            int iDate = result.Metadata.LastUpdated.Value.Year;
            iDate = iDate | (result.Metadata.LastUpdated.Value.Month << 16) | (result.Metadata.LastUpdated.Value.Day << 24);

            int iTime = result.Metadata.LastUpdated.Value.Hour;
            iTime = iTime | (result.Metadata.LastUpdated.Value.Minute << 8);

            var writer = new BinaryWriter(fs, Encoding.UTF8, false);
            writer.Write(iDate);
            writer.Write(iTime);

            // Конвертуємо лінії в полігони для експорту
            var allPolygons = new List<Polygon>(result.Polygons);
            foreach (var line in result.Lines)
            {
                allPolygons.Add(ConvertLineToPolygon(line));
            }

            foreach (var polygon in allPolygons)
            {
                iType = 0;
                if (polygon.Name.Contains("невідомий"))
                {
                    iType = 0x00010000;  // mean gray zone
                }
                else if (
                    !polygon.Name.Contains("Окуповано") &&
                    !polygon.Name.Contains("ОРДЛО") &&
                    !polygon.Name.Contains("Крим") &&
                    !polygon.Name.Contains("assessed_russian_advance") &&
                    !polygon.Name.Contains("RU occupied"))
                {
                    continue;
                }

                iCount = polygon.Points.Count;
                iCount = iCount | iType;
                writer.Write(iCount);

                foreach (var point in polygon.Points)
                {
                    writer.Write(point.Lat);
                    writer.Write(point.Lng);
                }
            }

            fs?.Close();
        }

        /// <summary>
        /// Конвертує лінію в полігон шляхом дублювання точок у зворотному порядку
        /// </summary>
        private static Polygon ConvertLineToPolygon(Line line)
        {
            var polygon = new Polygon
            {
                Name = line.Name,
                Style = line.Style
            };

            // Додаємо точки в прямому порядку
            foreach (var point in line.Points)
            {
                polygon.Points.Add(point);
            }

            // Додаємо точки в зворотному порядку (крім останньої, щоб не дублювати)
            for (int i = line.Points.Count - 2; i >= 0; i--)
            {
                polygon.Points.Add(line.Points[i]);
            }

            return polygon;
        }

        /// <summary>
        /// Перевіряє чи запущена програма з правами адміністратора
        /// </summary>
        private static bool IsRunAsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Перезапускає програму з правами адміністратора
        /// </summary>
        private static void RestartAsAdministrator()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Application.ExecutablePath,
                    Verb = "runas"
                };

                Process.Start(startInfo);

                // Закриваємо поточну програму
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося перезапустити програму з правами адміністратора: {ex.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}