using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moonmile.XFormsProvider
{
    public static class PageExtentions
    {
        /// <summary>
        /// Xamarin.Forms.FindByName を変更する拡張メソッド
        /// </summary>
        /// <param name="page"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object FindByName(this Xamarin.Forms.Page page, string name)
        {
            return PageXaml.FindByName(page, name);
        }
        public static T FindByName<T>(this Xamarin.Forms.Page page, string name) where T : class
        {
            return page.FindByName(name) as T;
        }
    }
}
