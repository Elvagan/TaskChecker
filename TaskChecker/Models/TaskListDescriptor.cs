using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TaskChecker.Models
{
    public class TaskListDescriptor
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [XmlAttribute(AttributeName = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        [XmlAttribute(AttributeName = "ModificationDate")]
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Gets or sets the task list.
        /// </summary>
        [XmlArray(ElementName = "Tasks")]
        public List<Models.Task> Tasks { get; set; }
    }
}