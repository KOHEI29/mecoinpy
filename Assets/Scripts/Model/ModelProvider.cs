using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace mecoinpy
{
    public static class ModelProvider
    {
        private static HashSet<IModel> models = new HashSet<IModel>();

        public static T Get<T>(BaseModelFactory<T> modelFactory) where T: IModel
        {
            if(models.OfType<T>().FirstOrDefault() is T model)
            {
                return model;
            }
            else
            {
                var newModel = modelFactory.Create();
                models.Add(newModel);
                return newModel;
            }
        }
        public static T Get<T>()
        {
            if(models.OfType<T>().FirstOrDefault() is T model)
            {
                return model;
            }
            else
            {
                return default;
            }
        }

        public static void Clear()
        {
            models.Clear();
        }
    }
}
