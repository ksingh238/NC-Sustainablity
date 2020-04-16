using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCSustainablity.Models
{
    public static class GlobalData
    {
        static bool _lockedin;
        static string _email;
        static int _Subscribe;
        static int _SurveyNo;
        static GlobalData()
        {
            _lockedin = new bool();
            _Subscribe = new int();
            _SurveyNo = new int();
        }
        public static bool IsLoggedIn()
        {
            return _lockedin;
        }
        public static void SetLoginStatus(bool value)
        {
            _lockedin = value;
        }
        public static string GetEmail()
        {
            return _email;
        }
        public static void SetEmail(string Email)
        {
            _email = Email;
        }
        public static int IsSubscribed()
        {
            return _Subscribe;
        }
        public static void SetSubscribeStatus(int value)
        {
            _Subscribe = value;
        }

        public static int GetCurrentSurvey()
        {
            return _SurveyNo;
        }
        public static void ResetSurveyCount()
        {
            _SurveyNo = 0;
        }
    }
}