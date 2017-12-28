using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FirstRunSlider.Helpers
{
    class PreferencesHelper
    {
        ISharedPreferences pref;
        ISharedPreferencesEditor editor;
        Context _context;

        // Shared preferences file name
        private static string PREF_NAME = "FirstRunSliderPreferences";
 
        private static string IS_FIRST_TIME_LAUNCH = "IsFirstTimeLaunch";
 
        public PreferencesHelper(Context context)
        {
            this._context = context;
            pref = _context.GetSharedPreferences(PREF_NAME, FileCreationMode.Private);
            editor = pref.Edit();
        }

        public void SetFirstTimeLaunch(bool isFirstTime)
        {
            editor.PutBoolean(IS_FIRST_TIME_LAUNCH, isFirstTime);
            editor.Commit();
        }

        public bool IsFirstTimeLaunch()
        {
            return true; // pref.GetBoolean(IS_FIRST_TIME_LAUNCH, true);
        }
    }
}