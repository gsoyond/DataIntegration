using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataIntegration.util
{
    /// <summary>
    /// 使用SqlBulkCopy实现数据的批量拷贝
    /// </summary>
    public class DataBulkCopy
    {
        /// sql bulk copy注意需要列的顺序是对应的，如不是，一种方法是在取数据时就按目标表的列顺序来；
        /// 另一种是配置列的对应关系ColumnMappings。
        /// 此外，源和目标的列个数可以不一样，但像上面说的，顺序最好保持一致。
        

    }
}
