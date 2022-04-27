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
        public static async Task<bool> SaveCoinList(List<Coin> coins, string file)
        {
            // If this is a new list
            if (!File.Exists(file))
                return await WriteToCoinListFile(file, coins); // Save the new list to a new file


            // Incase the file is locked, automatically retry 10 times, waiting 1 second between retries
            int x = 0;
            do
            {
                // If the file exists and its not locked
                if (File.Exists(file) && !IsFileLocked(new FileInfo(file)))
                    return await WriteToCoinListFile(file, coins);  // Save the updated list to the file

                x++;
                await Task.Delay(1000);
            } while (x < 10);

            try
            {
                // The file is locked, Try deleting the file
                File.Delete(file);
                return await WriteToCoinListFile(file, coins);  // Then save the updated list to the new file
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
        public static async Task<List<Coin>> LoadCoinList(string file)
        {
            // Incase the file is locked, automatically retry 10 times, waiting 1 second between retries
            int x = 0;
            do
            {
                if (File.Exists(file) && !IsFileLocked(new FileInfo(file)))
                {
                    try
                    {
                        // Open the file
                        using FileStream fs = File.OpenRead(file);
                        // Extract the data
                        var coins = await JsonSerializer.DeserializeAsync<List<Coin>>(fs);
                        // Close the file
                        fs.Close();

                        // Return the data if the file indeed has data
                        if (coins != null && coins.Count > 0)
                            return coins;
                    }
                    catch { }

                    // If the file has no data then return an empty list
                    return new List<Coin>();
                }
                x++;
                await Task.Delay(1000);
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
        public static async Task<bool> SaveGpuList(List<Gpu> gpus, string file)
        {

            // If this is a new list
            if (!File.Exists(file))                
                return await WriteToGpuListFile(file, gpus); // Save the new list to a new file


            // Incase the file is locked, automatically retry 10 times, waiting 1 second between retries
            int x = 0;
            do
            {
                // If the file exists and its not locked
                if (File.Exists(file) && !IsFileLocked(new FileInfo(file)))                
                    return await WriteToGpuListFile(file, gpus);  // Save the updated list to the file

                x++;
                await Task.Delay(1000);
            } while (x < 10);

            try
            { 
                // The file is locked, Try deleting the file
                File.Delete(file);
                return await WriteToGpuListFile(file, gpus);  // Then save the updated list to the new file
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
        public static async Task<List<Gpu>> LoadGpuList(string file)
        {
            // Incase the file is locked, automatically retry 10 times, waiting 1 second between retries
            int x = 0;
            do
            {
                if (File.Exists(file) && !IsFileLocked(new FileInfo(file)))
                {
                    try
                    {
                        // Open the file
                        using FileStream fs = File.OpenRead(file);
                        // Extract the data
                        var gpus = await JsonSerializer.DeserializeAsync<List<Gpu>>(fs);
                        // Close the file
                        fs.Close();

                        // Return the data if the file indeed has data
                        if (gpus != null && gpus.Count > 0)
                            return gpus;
                    }
                    catch { }

                    // If the file has no data then return an empty list
                    MessageBox.Show("Failed to load Gpu List data! Please import a backup.");
                    return new List<Gpu>();
                }
                x++;
                await Task.Delay(1000);
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
        public static async Task<bool> WriteToGpuListFile(string file, List<Gpu> gpus)
        {
            // Save the given gpu list to the given file
            try
            {
                using FileStream createStream = File.Create(file);
                await JsonSerializer.SerializeAsync(createStream, gpus, new JsonSerializerOptions() { WriteIndented = true });
                createStream.Close();
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
        public static async Task<bool> WriteToCoinListFile(string file, List<Coin> coins)
        {
            // Save the given coin list to the given file
            try
            {
                using FileStream createStream = File.Create(file);
                await JsonSerializer.SerializeAsync(createStream, coins, new JsonSerializerOptions() { WriteIndented = true });
                createStream.Close();
                return true;
            }
            catch { return false; }
        }
    }
}
