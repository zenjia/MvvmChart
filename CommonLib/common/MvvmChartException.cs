using System;

namespace MvvmCharting.Common
{
    public class MvvmChartException : Exception
    {
        public static MvvmChartException ThrowDataTemplateRootElementException(string dataTemplateName, Type baseType)
        {
            throw new MvvmChartException($"The root element in the {dataTemplateName} should inherit from {baseType}.");
        }

 

        public MvvmChartException(string msg)
            :base(msg)
        {
           
        }

    }
}