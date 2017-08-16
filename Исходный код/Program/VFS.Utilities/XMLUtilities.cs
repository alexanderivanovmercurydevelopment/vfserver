namespace VFS.Utilities
{
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Вспомогательные методы сериализации/десериализации xml.
    /// </summary>
    public static class XMLUtilities
    {
        /// <summary>
        /// Сериализовать объект в XML.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="xml">Объект.</param>
        /// <returns>Строка XML, в которую преобразован объект.</returns>
        public static T DeserializeFromXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (TextReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Сериализовать объект в xml.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="object">Объект.</param>
        /// <returns>XML-представление объекта.</returns>
        public static string SerializeToXml<T>(T @object)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, @object);
                return stringWriter.ToString(); // Your XML
            }
        }

        /// <summary>
        /// Проверить соответствие xml схеме xsd.
        /// </summary>
        /// <param name="xml">XML-содержимое.</param>
        /// <param name="xsd">XSD-схема.</param>
        /// <returns>True - валидация прошла успешно, false - нет.</returns>
        public static bool ValidateXml(string xml, string xsd)
        {
            bool result = true;

            XmlSchemaSet schemas = new XmlSchemaSet();

            using (TextReader reader = new StringReader(xsd))
            {
                XmlSchema schema = XmlSchema.Read(
                    reader,
                    (o, e) => { result = false; });

                schemas.Add(schema);
            }            

            XDocument doc = XDocument.Parse(xml);

            doc.Validate(schemas, (o, e) =>
            {
                result = false;
            });
            
            return result;
        }
    }
}
