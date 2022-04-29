using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Max_Gpu_Roi
{
    internal class JsonCrud
    {
        /// <summary>
        /// Save a given coin list to a file in json
        /// </summary>
        /// <param name="coins"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        public static bool SaveCoinList(List<Coin> coins, string file)
        {
            // If this is a new list
            if (!File.Exists(file))
                return WriteToCoinListFile(file, coins); // Save the new list to a new file


            // Incase the file is locked, automatically retry 10 times, waiting 1 second between retries
            int x = 0;
            do
            {
                // If the file exists and its not locked
                if (File.Exists(file) && !IsFileLocked(new FileInfo(file)))
                    return WriteToCoinListFile(file, coins);  // Save the updated list to the file

                x++;
                Task.Delay(1000);
            } while (x < 10);

            try
            {
                // The file is locked, Try deleting the file
                File.Delete(file);
                return WriteToCoinListFile(file, coins);  // Then save the updated list to the new file
            }
            catch { }

            // Otherwise the file is in-use and cannot be deleted so we let the user know with a popup
            var message = file + "  - Coin list file in-use, please make sure the file isn't open and try again.";
            MessageBox.Show(message);
            return false;
        }
        
        /// <summary>
        /// Load a given coin list from a json file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<Coin> LoadCoinList(string file)
        {
            // Incase the file is locked, automatically retry 10 times, waiting 1 second between retries
            int x = 0;
            do
            {
                if (File.Exists(file) && !IsFileLocked(new FileInfo(file)))
                {
                    try
                    {
                        string jsonString = File.ReadAllText(file);

                        // Extract the data
                        var coins = JsonSerializer.Deserialize<List<Coin>>(jsonString);

                        // Return the data if the file indeed has data
                        if (coins != null && coins.Count > 0)
                            return coins;
                    }
                    catch { }

                    // If the file has no data then return an empty list
                    return new List<Coin>();
                }
                x++;
                Task.Delay(1000);
            } while (x < 10);

            // The file was not found or in-use. Show a pop-up with which one it is.
            var message = "";
            if (!File.Exists(file))
                message = file + "  - Coin list file not found!";
            else
                message = file + "  - Coin list file in-use, please make sure the file isn't open and try again.";
            MessageBox.Show(message);
            return new List<Coin>();
        }

        /// <summary>
        /// Save a given gpu list to a file in json
        /// </summary>
        /// <param name="gpus"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        public static bool SaveGpuList(List<Gpu> gpus, string file)
        {

            // If this is a new list
            if (!File.Exists(file))                
                return WriteToGpuListFile(file, gpus); // Save the new list to a new file


            // Incase the file is locked, automatically retry 10 times, waiting 1 second between retries
            int x = 0;
            do
            {
                // If the file exists and its not locked
                if (File.Exists(file) && !IsFileLocked(new FileInfo(file)))                
                    return WriteToGpuListFile(file, gpus);  // Save the updated list to the file

                x++;
                Task.Delay(1000);
            } while (x < 10);

            try
            { 
                // The file is locked, Try deleting the file
                File.Delete(file);
                return WriteToGpuListFile(file, gpus);  // Then save the updated list to the new file
            }
            catch { }

            // Otherwise the file is in-use and cannot be deleted so we let the user know with a popup
            var message = file + "  - Gpu list file in-use, please make sure the file isn't open and try again.";
            MessageBox.Show(message);
            return false;
        }

        /// <summary>
        /// Load a given gpu list as a json file
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public static List<Gpu> LoadGpuList(string file)
        {
            // Incase the file is locked, automatically retry 10 times, waiting 1 second between retries
            int x = 0;
            do
            {
                if (File.Exists(file) && !IsFileLocked(new FileInfo(file)))
                {
                    try
                    {
                        // Read the file
                        string jsonString = File.ReadAllText(file); 
                        
                        // Extract the data
                        var gpus = JsonSerializer.Deserialize<List<Gpu>>(jsonString);

                        // Return the data if the file indeed has data
                        if (gpus != null && gpus.Count > 0)
                            return gpus;
                    }
                    catch { }

                    // If the file has no data then return an empty list
                    MessageBox.Show("Failed to load Gpu List data! Please import a backup.");
                    return new List<Gpu>();
                }
                // Wait a second before retrying to open the file that is in-use
                x++;
                Task.Delay(1000);
            } while (x < 10);

            // The file was not found or in-use. Show a pop-up with which one it is.
            var message = "";
            if (!File.Exists(file))
                message = file + "  - Gpu list file not found!";
            else
                message = file + "  - Gpu list file in-use, please make sure the file isn't open and try again.";
            MessageBox.Show(message);
            return new List<Gpu>();
        }

        /// <summary>
        /// Check if a file is locked
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        /// <summary>
        /// Write a given gpu list to a given file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="gpus"></param>
        /// <returns></returns>
        public static bool WriteToGpuListFile(string file, List<Gpu> gpus)
        {
            // Save the given gpu list to the given file
            try
            {
                var jsonString = JsonSerializer.Serialize<List<Gpu>>(gpus, new JsonSerializerOptions() { WriteIndented = true });
                File.WriteAllText(file, jsonString);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Write a given coin list to a given file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="gpus"></param>
        /// <returns></returns>
        public static bool WriteToCoinListFile(string file, List<Coin> coins)
        {
            // Save the given coin list to the given file
            try
            {
                var jsonString = JsonSerializer.Serialize<List<Coin>>(coins, new JsonSerializerOptions() { WriteIndented = true });
                File.WriteAllText(file, jsonString);
                return true;
            }
            catch { return false; }
        }
    }
}
