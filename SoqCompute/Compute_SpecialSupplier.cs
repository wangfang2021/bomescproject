using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SoqCompute
{
    public class Compute_SpecialSupplier : Compute_Special
    {

        //平准特殊厂家主方法			
        public void pinZhun_SpecialSupplier(ref ArrayList beginData, DataTable dtCalendar, DataTable dtSpecialSupplier)
        {
            pinZhun_Special(ref beginData, dtCalendar, dtSpecialSupplier, "特殊厂家");
        }
        

    }
}
