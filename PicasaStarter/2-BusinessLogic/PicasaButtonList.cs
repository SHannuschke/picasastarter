﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;              // For manipulating the registry...
using HelperClasses.Logger;

namespace PicasaStarter
{
    public class PicasaButtons
    {
        [NonSerialized]
        public List<PicasaButton> ButtonList = new List<PicasaButton>();

        public void Registerbuttons()
        {
            // Retrieve buttons that are not shown in Picasa, but are registered already...
            List<string> hiddenButtonIDs = new List<string>();
            string keyNameHiddenButtons = "Software\\Google\\Picasa\\Picasa2\\Preferences\\Buttons\\Exclude";
            
            RegistryKey keyHiddenButtons = Registry.CurrentUser.OpenSubKey(keyNameHiddenButtons);
            if (keyHiddenButtons != null) {
                string[] hiddenIndexes = keyHiddenButtons.GetValueNames();

                for (int i = 0; i < hiddenIndexes.Length; i++)
                {
                    hiddenButtonIDs.Add(keyHiddenButtons.GetValue(hiddenIndexes[i]).ToString());
                }
            }

            // Retrieve buttons that are shown already in Picasa
            List<string> shownButtonIDs = new List<string>();
            string keyNameShownButtons = "Software\\Google\\Picasa\\Picasa2\\Preferences\\Buttons\\UserConfig";
            int highestShownIndex = 0;

            RegistryKey keyShownButtons = Registry.CurrentUser.OpenSubKey(keyNameShownButtons);
            if (keyShownButtons != null) {
                string[] shownIndexes = keyShownButtons.GetValueNames();
                int index = 0;

                for (int i = 0; i < shownIndexes.Length; i++) {
                    shownButtonIDs.Add(keyShownButtons.GetValue(shownIndexes[i]).ToString());

                    // Find the highest index
                    int.TryParse(shownIndexes[i], out index);
                    if (index > highestShownIndex)
                        highestShownIndex = index;
                }
            }

            // Loop over all buttons and register them if needed...
            foreach (PicasaButton button in ButtonList)
            {
                // If the ButtonID doesn't exist in the registry, add...
                if (hiddenButtonIDs.Contains(button.ButtonID))
                    continue;
                else if (!shownButtonIDs.Contains(button.ButtonID))
                    Registry.SetValue("HKEY_CURRENT_USER\\" + keyNameShownButtons, (++highestShownIndex).ToString(), button.ButtonID);
            }
        }
    }
}
