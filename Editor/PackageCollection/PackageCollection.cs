using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    [Serializable]
    public class PackageCollection
    {
        public string collectionName;
        [Tooltip("Список git-ссылок (например, https://github.com)")]
        public List<string> gitUrls = new List<string>();

        // Убедитесь, что здесь НЕТ [NonSerialized]. HideInInspector скрывает только в обычном инспекторе, но сохранению не мешает.
        [HideInInspector] public bool isSelected;
    }
}
