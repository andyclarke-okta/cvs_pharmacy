using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    public class SCIMFilter
    {
        public static string[] or_separator = new string[] { " OR ", " Or ", " or " };
        public static string[] eq_separator = new string[] { " EQ ", " Eq ", " eq " };

        public static SCIMFilterAttribute buildAttribute(string name)
        {
            SCIMFilterAttribute sfa = new SCIMFilterAttribute();

            string[] names = name.Split(new string[] { "." }, StringSplitOptions.None);
            sfa.AttributeName = names[0];

            // check for custom attributes...
            // check for sub attributes
            // what about multiple sub attributes eg. x.y.z
            if (names.Length > 1)
            {
                sfa.SubAttributeName = names[1];
            }

            return sfa;
        }
        public static SCIMFilter buildExpression(string expression)
        {
            SCIMFilter sf = new SCIMFilter();
            string[] splits = expression.Split(or_separator, StringSplitOptions.None);
            if (splits.Length == 1)
            {
                string[] name_value = splits[0].Split(eq_separator, StringSplitOptions.None);
                sf.FilterAttribute = buildAttribute(name_value[0]);
                sf.FilterValue = name_value[1];
                sf.FilterType = SCIMFilterType.EQUALS;
            }
            else
            {
                sf.FilterType = SCIMFilterType.OR;
                sf.FilterExpressions = new List<SCIMFilter>();
                foreach (string exp in splits)
                {
                    sf.FilterExpressions.Add(buildExpression(exp));
                }
            }
            return sf;
        }

        // Parse a string into a SCIMFilter Object
        public static SCIMFilter TryParse(string input)
        {
            return buildExpression(input);
        }
        public SCIMFilterAttribute FilterAttribute { get; set; }

        public List<SCIMFilter> FilterExpressions { get; set; }

        public SCIMFilterType FilterType { get; set; }

        public String FilterValue { get; set; }
    }
}
