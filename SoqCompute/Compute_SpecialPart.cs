using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SoqCompute
{
    public class Compute_SpecialPart : Compute_Special
    {
        //平准特殊品番主方法	
        public void pinZhun_SpecialPart(ref ArrayList beginData, DataTable dtCalendar, DataTable dtSpecialPart)
        {
            pinZhun_Special(ref beginData, dtCalendar, dtSpecialPart, "特殊品番");
        }


    }
}
