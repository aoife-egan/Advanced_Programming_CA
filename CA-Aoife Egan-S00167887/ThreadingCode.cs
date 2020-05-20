using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows;
using System.Threading;
using System.Windows.Media;

namespace CA_Aoife_Egan_S00167887
{
    class ThreadingCode
    {
        // synchronisation obj
        public static Object synObj = new Object();

        //create isolated storage for user & assembly
        private IsolatedStorageFile store;

        // folder name
        private string folderName;

        //path to file
        private string pathToFile;

        // Create a new Mutex.
        private static Mutex mut = new Mutex();

        //constructor method to create isolated storage
        public ThreadingCode()
        {
            //give folder a name 
            folderName = "Theme Folder";
            //set path to file
            pathToFile = String.Format("{0}\\ThemeFile.txt", folderName);
            // set storage type & access store
            store = IsolatedStorageFile.GetUserStoreForAssembly();          
        }

        //method to write selected theme to ThemeFile.txt
        public void writeToStorage(Object themeFromUser)
        {
            string theme = themeFromUser.ToString();

            //check store exists
            if (store != null)
            {
                //synchronise access to isolated storage text file
                //lock (synObj) 
                MessageBox.Show(Thread.CurrentThread.Name+" is requesting the mutex");
                mut.WaitOne();
                MessageBox.Show(Thread.CurrentThread.Name + " has entered the protected area");
                try
                    {
                        //check folder exists or if not create one
                        if (!store.DirectoryExists(folderName))
                        {
                            store.CreateDirectory(folderName);
                        }
                        //create isolated storage file 
                        using (IsolatedStorageFileStream isolatedStorageFile =
                            store.OpenFile(pathToFile, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(isolatedStorageFile))
                            {
                                writer.Write(theme);
                                MessageBox.Show(Thread.CurrentThread.Name + " is writing to ThemeFile.txt in isolated storage");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);

                    }
                    finally
                    {
                        readFromStorage();
                    MessageBox.Show(Thread.CurrentThread.Name + " is leaving the protected area");
                    // Release the Mutex.
                    mut.ReleaseMutex();
                    MessageBox.Show(Thread.CurrentThread.Name + " has released the mutex");
                }
                //}
            }
        }


        //method to read theme from file in isolated storage
        public void readFromStorage()
        {
            if (store != null)
            {
                try
                {
                    //synchronise access to isolatd storage
                    lock (synObj)
                    {
                        //read from txt file 
                        using (IsolatedStorageFileStream isolatedStorageFile =
                            store.OpenFile(pathToFile, FileMode.Open, FileAccess.Read))
                        {
                            using (StreamReader reader = new StreamReader(isolatedStorageFile))
                            {
                                string themeFromFile = reader.ReadLine();

                                MessageBox.Show(Thread.CurrentThread.Name + " is reading theme from isolated storage & applying!");
                                //apply theme
                                MainWindow.mainWindow.Dispatcher.Invoke(new Action(
                                    delegate ()
                                    {
                                        MainWindow.mainWindow.applyTheme(themeFromFile);
                                    }));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
         
    }
}
