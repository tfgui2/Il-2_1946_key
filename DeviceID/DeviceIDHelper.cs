using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceIDLibrary
{
    public class DeviceIDHelper
    {
        private IList<SharpDX.DirectInput.DeviceInstance> _list;
        private void UpdateList()
        {
            var di = new SharpDX.DirectInput.DirectInput();

            var oldlist = _list;
            _list = di.GetDevices(SharpDX.DirectInput.DeviceClass.GameControl, SharpDX.DirectInput.DeviceEnumerationFlags.AttachedOnly);
            IsChanged = CheckChanged(oldlist, _list);
        }

        public bool IsChanged { get; set; }

        public List<string> GetDeviceList()
        {
            UpdateList();

            List<string> res = new List<string>();
            foreach (var item in _list)
            {
                res.Add(item.ProductName);
            }

            return res;
        }

        private bool CheckChanged(IList<SharpDX.DirectInput.DeviceInstance> oldlist, IList<SharpDX.DirectInput.DeviceInstance> newlist)
        {
            if (oldlist == null || newlist == null)
                return true;

            if (oldlist.Count != newlist.Count)
                return true;

            for (int i = 0; i < oldlist.Count; i++)
            {
                var item1 = oldlist[i];
                var item2 = newlist[i];

                if (item1.ProductGuid != item2.ProductGuid)
                    return true;
            }

            return false;
        }

    }
}
