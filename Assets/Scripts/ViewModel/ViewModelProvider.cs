using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace mecoinpy
{
    //ViewModelのProvider
    //ToDo.キャッシュ機能付けたい
    public static class ViewModelProvider
    {
        private static HashSet<IViewModel> viewModels = new HashSet<IViewModel>();

        public static T Get<T>(BaseViewModelFactory<T> factory, bool cache = false) where T : BaseViewModel{
            if(viewModels.OfType<T>().FirstOrDefault() is T viewModel)
            {
                return viewModel;
            }
            else
            {
                var newViewModel = factory.Create();
                if(cache)
                    viewModels.Add(newViewModel);
                return newViewModel;
            }
            
        }

        public static void Clear()
        {
            viewModels.Clear();
        }
    }
}