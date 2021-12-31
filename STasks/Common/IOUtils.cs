using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Common
{
    public static class IOUtils
    {
        /// <summary>
        /// returns the selection path, or "" on cancelation
        /// </summary>
        /// <param name="ext">the default extension</param>
        /// <returns></returns>
        public static string PromptSavingPath(string ext)
        {
            Ookii.Dialogs.Wpf.VistaSaveFileDialog vsfd = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
            vsfd.DefaultExt = ext;
            bool? notcanceled= vsfd.ShowDialog();

            if (notcanceled.HasValue == false || notcanceled.Value == false) return"";
            if (string.IsNullOrWhiteSpace(vsfd.FileName) == false)
            {
                return vsfd.FileName;
            }
            else return "";
        }

        /// <summary>
        /// if not canceled, performs the ction delegateagainst the selected path
        /// NOTE: the boolean param is true if the selection was canceled, 
        /// NOTE: runs async
        /// </summary>
        /// <param name="ext">the default extension</param>
        /// <returns></returns>
        public static void PromptSavingPathAsync(string ext, Action<string,bool>CallbackAction)
        {
            Task.Run(() =>
            {

                Ookii.Dialogs.Wpf.VistaSaveFileDialog vsfd = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
                vsfd.DefaultExt = ext;
                bool? notcanceled = vsfd.ShowDialog();

                if (notcanceled.HasValue == false || notcanceled.Value == false) {
                    CallbackAction(vsfd.FileName, true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(vsfd.FileName) == false)
                {
                    CallbackAction(vsfd.FileName, false);
                    return;
                }

                else {
                    CallbackAction("", true);
                    return;
                } 
            });
            
        }

        internal static void InvokeItemAsync(string absolutePath)
        {
            Task.Run(() => {
                if(File.Exists(absolutePath))
                Process.Start(absolutePath);
            });
        }
    }
}
