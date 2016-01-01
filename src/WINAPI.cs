using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Security;

namespace VIANetWorkCard
{
    class WINAPI
    {
        private const int ERROR_ELEVATION_REQUIRED = 740;
        private static Bitmap shield_bm = null;

        [DllImport("user32")]
        public static extern UInt32 SendMessage
        (IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CreateProcess(string lpApplicationName,
           string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
           ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles,
           CreationFlags dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
           [In] ref STARTUPINFO lpStartupInfo,
           out PROCESS_INFORMATION lpProcessInformation);

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [Flags]
        private enum CreationFlags : uint
        {CREATE_SUSPENDED = 0x4}

        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon", CharSet = CharSet.Auto)]
            internal static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
        }

        internal const int BCM_FIRST = 0x1600; //Normal button
        internal const int BCM_SETSHIELD = (BCM_FIRST + 0x000C); //Elevated button

        public static string GetShortcutTargetFile(string shortcutFilename)
        {
            if (System.IO.File.Exists(shortcutFilename))
            {
                try
                {
                WshShell shell = new WshShell(); //Create a new WshShell Interface
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcutFilename); //Link the interface to our shortcut
                return link.TargetPath; //Show the target in a MessageBox using IWshShortcut
                }catch(Exception e)
                {
                    Console.WriteLine(e);
                    return shortcutFilename;
                }
            }
            return null;
        }

        internal string GetSystemDefaultBrowser()
        {
            string name = string.Empty;
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.ClassesRoot.OpenSubKey("HTTP\\shell\\open\\command", false);
                name = regKey.GetValue(null).ToString().Replace("" + (char)34, "");
                if (!name.EndsWith("exe"))
                {
                    name = name.Substring(0, name.LastIndexOf(".exe") + 4);
                }
            }
            catch (Exception ex)
            {
                name = string.Format("ERROR: An exception of type: {0} occurred in method: {1} in the following module: {2}", ex.GetType(), ex.TargetSite, this.GetType());
            }
            finally
            {
                if (regKey != null)
                {
                    regKey.Close();
                }
            }
            return name;
        }

        public static void AddShieldToButton(Button b)
        {
            b.FlatStyle = FlatStyle.System;
            SendMessage(b.Handle, BCM_SETSHIELD, 0, 0xFFFFFFFF);
        }

        public static void RemoveShieldFromButton(Button b)
        {
            b.FlatStyle = FlatStyle.System;
            SendMessage(b.Handle, BCM_SETSHIELD, 0, 0x0);
        }

        public static bool RequiresElevation(string Filename)
        {
            bool requiresElevation;
            bool success;
            int win32error;

            PROCESS_INFORMATION pInfo = new PROCESS_INFORMATION();
            STARTUPINFO sInfo = new STARTUPINFO();
            SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
            SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();

            pSec.nLength = Marshal.SizeOf(pSec);
            tSec.nLength = Marshal.SizeOf(tSec);

            success = CreateProcess(null, Filename,
                ref pSec, ref tSec, false, CreationFlags.CREATE_SUSPENDED,
                IntPtr.Zero, null, ref sInfo, out pInfo);

            if (success)
            {
                requiresElevation = false;
            }else{
                win32error = Marshal.GetLastWin32Error();
                if (win32error == ERROR_ELEVATION_REQUIRED)
                    requiresElevation = true;
                else
                    throw new Win32Exception(win32error);
            }
            TerminateProcess(pInfo.hProcess, 0);
            CloseHandle(pInfo.hThread);
            CloseHandle(pInfo.hProcess);
            return requiresElevation;
        }

        public static Icon ExtractAssociatedIcon(String filePath)
        {
            int index = 0;

            Uri uri;

            if (filePath == null)
            {
                throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", "null", "filePath"), "filePath");
            }
            try
            {
                uri = new Uri(filePath);
            }
            catch (UriFormatException)
            {
                filePath = Path.GetFullPath(filePath);
                uri = new Uri(filePath);
            }

            if (uri.IsFile)
            {
                if (!System.IO.File.Exists(filePath))
                {
                    throw new FileNotFoundException(filePath);
                }
                StringBuilder iconPath = new StringBuilder(260);
                iconPath.Append(filePath);
                IntPtr handle = SafeNativeMethods.ExtractAssociatedIcon(new HandleRef(null, IntPtr.Zero), iconPath, ref index);
                if (handle != IntPtr.Zero)
                {
                    return Icon.FromHandle(handle);
                }
            }
            return null;
        }
        public static Bitmap GetUacShieldImage()
        {
            if (shield_bm != null) return shield_bm;

            const int WID = 50;
            const int HGT = 50;
            const int MARGIN = 4;

            // Make the button. For some reason, it must
            // have text or the UAC shield won't appear.
            Button btn = new Button();
            btn.Text = " ";
            btn.Size = new Size(WID, HGT);
            AddShieldToButton(btn);

            // Draw the button onto a bitmap.
            Bitmap bm = new Bitmap(WID, HGT);
            btn.Refresh();
            btn.DrawToBitmap(bm, new Rectangle(0, 0, WID, HGT));

            // Find the part containing the shield.
            int min_x = WID, max_x = 0, min_y = HGT, max_y = 0;

            // Fill on the left.
            for (int y = MARGIN; y < HGT - MARGIN; y++)
            {
                // Get the leftmost pixel's color.
                Color target_color = bm.GetPixel(MARGIN, y);

                // Fill in with this color as long as we see the target.
                for (int x = MARGIN; x < WID - MARGIN; x++)
                {
                    // See if this pixel is part of the shield.
                    if (bm.GetPixel(x, y).Equals(target_color))
                    {
                        // It's not part of the shield.
                        // Clear the pixel.
                        bm.SetPixel(x, y, Color.Transparent);
                    }
                    else
                    {
                        // It's part of the shield.
                        if (min_y > y) min_y = y;
                        if (min_x > x) min_x = x;
                        if (max_y < y) max_y = y;
                        if (max_x < x) max_x = x;
                    }
                }
            }

            // Clip out the shield part.
            int shield_wid = max_x - min_x + 1;
            int shield_hgt = max_y - min_y + 1;
            shield_bm = new Bitmap(shield_wid, shield_hgt);
            Graphics shield_gr = Graphics.FromImage(shield_bm);
            shield_gr.DrawImage(bm, 0, 0,
                new Rectangle(min_x, min_y, shield_wid, shield_hgt),
                GraphicsUnit.Pixel);

            // Return the shield.
            return shield_bm;
        }

    }
}
