using System;
using System.Collections.Generic;
using System.Text;

namespace SMPL.Reporters
{
    interface IReporter
    {
        void GetReport(string filename = null);
    }
}
