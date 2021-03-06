﻿using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Compares string in case insensetive mode
    /// </summary>
    public class IgnoreCaseCriterion : ICriterion
    {
        private readonly ICriterion _subCriterion;

        /// <summary>
        /// Creates new instance of <see cref="IgnoreCaseCriterion"/>
        /// </summary>
        /// <param name="subCriterion">criterion to apply data to</param>
        public IgnoreCaseCriterion(ICriterion subCriterion)
        {
            _subCriterion = subCriterion;
        }

        public void WriteTo(JsonWriter writer)
        {
            //$icase Case insensitive string matching:
            //{'fpath' : {'$icase' : 'val1'}} //icase matching
            //Ignore case matching with '$in' operation:
            //{'name' : {'$icase' : {'$in' : ['théâtre - театр', 'hello world']}}}
            //For case insensitive matching you can create special index of type: `JBIDXISTR`

            writer.WriteObjectBasedCriterion("$icase", _subCriterion);
        }
    }
}