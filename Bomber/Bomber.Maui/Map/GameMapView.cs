using System;
using Bomber.Game.Visuals.Views;
using GameFramework.UI.Maui.Map;
using GameFramework.Visuals.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Bomber.Maui.Map
{
    internal class GameMapView : MauiMapControl, IGameMapView
    {
        public void PlantBomb(IMovingObjectView bombView)
        {
            throw new NotImplementedException();
        }

        public void DeleteBomb(IMovingObjectView bombView)
        {
            throw new NotImplementedException();
        }
        
        public override void OnViewDisposed(IDisposableStaticObjectView view)
        {
            if (view is BoxView shape)
            {
                if (MainThread.IsMainThread)
                {
                    Children.Remove(shape);
                }
                else
                {
                    MainThread.InvokeOnMainThreadAsync(() => Children.Remove(shape));
                }
            }
        }

        protected override void UpdateEntities()
        {
            if (MainThread.IsMainThread)
            {
                UpdateEntitiesOp();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(UpdateEntitiesOp);
            }
        }

        private void UpdateEntitiesOp()
        {
            foreach (var entityView in DisposableObjectViews)
            {
                if (entityView is BoxView shape && !Children.Contains(shape))
                {
                    Children.Add(shape);
                    entityView.Attach(this);
                }
            }
        }
        
        protected override void UpdateMapObjects()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var mapObject in MapObjects)
                {
                    if (mapObject.View is BoxView shape && !Children.Contains(shape))
                    {
                        Children.Add(shape);
                    }
                }
            });
        }

        public override void Clear()
        {
            MainThread.BeginInvokeOnMainThread(() => Children.Clear());
        }
    }
}