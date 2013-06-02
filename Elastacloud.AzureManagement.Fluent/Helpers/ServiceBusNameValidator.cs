using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    public class ServiceBusNameValidator
    {
        private readonly string _name;

        public ServiceBusNameValidator(string name)
        {
            _name = name;
        }

        public bool ValidateName()
        {
            return CheckRegex() && CheckEnd() && CheckSize() && CheckFirstLetter();
        }

        private bool CheckRegex()
        {
            const string pattern = "^[a-zA-Z][a-zA-Z0-9-]*$";
            return Regex.IsMatch(_name, pattern);
        }

        private bool CheckEnd()
        {
            string lowerName = _name.ToLower();
            return !(lowerName.EndsWith("-") || lowerName.EndsWith("-sb") || lowerName.EndsWith("-mgmt") ||
                   lowerName.EndsWith("-cache")
                   || lowerName.EndsWith("-appfabric"));
        }

        private bool CheckSize()
        {
            return _name.Length >= 6 && _name.Length <= 50;
        }

        private bool CheckFirstLetter()
        {
            return Char.IsLetter(_name, 0);
        }
    }
}
