using System.Collections.Generic;

namespace HellEngine.Core.Models.GameControl
{
    public class SaveGame
    {
        public string UserName { get; set; }
        public string CurrentStateKey { get; set; }
        public IEnumerable<VarInfo> VarsInfo { get; set; }
    }
}
