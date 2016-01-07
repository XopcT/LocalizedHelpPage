using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LocalizedHelpPage.Controllers
{
    /// <summary>Localized values controller.</summary>
    /// <summary xml:lang="ru-RU">Контроллер с локализацией</summary>
    [Authorize]
    public class LocalizedValuesController : ApiController
    {
        /// <summary>Gets values</summary>
        /// <returns>Collection of values</returns>
        /// <summary xml:lang="ru-RU">Возвращает значения</summary>
        /// <returns xml:lang="ru-RU">Коллекция значений</returns>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>Gets a value</summary>
        /// <param name="id">ID of the value</param>
        /// <returns>Value</returns>
        /// <summary xml:lang="ru-RU">Возвращает значение</summary>
        /// <param name="id" xml:lang="ru-RU">ID значения</param>
        /// <returns xml:lang="ru-RU">Значение</returns>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>Sets a value</summary>
        /// <param name="value">Value to set</param>
        /// <summary xml:lang="ru-RU">Устанавливает значение</summary>
        /// <param name="value" xml:lang="ru-RU">Устанавливаемое значение</param>
        public void Post([FromBody]string value)
        {
        }

        /// <summary>Updates a value</summary>
        /// <param name="id">ID of the value</param>
        /// <param name="value">Value to set</param>
        /// <summary xml:lang="ru-RU">Изменяет значение</summary>
        /// <param name="id" xml:lang="ru-RU">ID значения</param>
        /// <param name="value" xml:lang="ru-RU">Устанавливаемое значение</param>
        public void Put(int id, [FromBody]string value)
        {
        }

        /// <summary>Deletes a value</summary>
        /// <param name="id">ID of the value</param>
        /// <summary xml:lang="ru-RU">Удаляет значение</summary>
        /// <param name="id" xml:lang="ru-RU">ID значения</param>
        public void Delete(int id)
        {
        }
    }
}
