using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

class ComparerTGSegmentDataType : IComparer<StopLocation>
{
    public int Compare(StopLocation x, StopLocation y)
    {
        int lvRes = -1;

        if (x == null) return -1;
        if (y == null) return 1;

        if(y.Start_coordinate >= x.Start_coordinate && y.End_coordinate <= x.End_coordinate)
        {
            lvRes = 0;
        }
        else if (x.Start_coordinate >= y.Start_coordinate)
        {
            lvRes = 1;
        }
        else if (x.Start_coordinate <= y.Start_coordinate)
        {
            lvRes = -1;
        }

        return lvRes;
    }
}
