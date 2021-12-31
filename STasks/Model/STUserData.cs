using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Model
{


    [Serializable]
    public struct BuildingBlockData
    {
        public DateTime CompletionDate { get;  set; }
        public Guid Guid { get; set; }
        public bool Value { get; set; }
    }
    /// <summary>
    /// contains the user progress and other info related to one specific semester
    /// </summary>
   [Serializable]
    public class STUserData
    {
        
        public string SemesterName { get; set; }
        public List<BuildingBlockData> BuildingBlocksCompletionArray { get;  set; }



        [NonSerialized]
        private Dictionary<Guid, BuildingBlockData> buildingBlocksData;
        



        static System.Xml.Serialization.XmlSerializer sr = new System.Xml.Serialization.XmlSerializer(typeof(STUserData));
        public static STUserData Load(string path)
        {
            using ( Stream s = File.OpenRead(path))
            {
                STUserData re = sr.Deserialize(s) as STUserData;
                re.buildingBlocksData = new Dictionary<Guid, BuildingBlockData>();
                foreach (var item in re.BuildingBlocksCompletionArray)
                {
                    re.buildingBlocksData.Add(item.Guid, item);
                }

                return re;
            }
        }

        public Dictionary<Guid, BuildingBlockData>  BuildingBlocksData()
        {
            return buildingBlocksData;
        }
        public void setBuildingBlocksData(Dictionary<Guid, BuildingBlockData> val)
        {
            buildingBlocksData = val;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="useArrayDirectly">passing true will serialize what's in the underlyng array without updating it with the disctionary, only do this if you imutated and want to save what's on the array , </param>
        public void Save(string path,bool useArrayDirectly=false)
        {
            using (Stream s = File.Open(path, FileMode.Create))
            {
                if (!useArrayDirectly)
                {
                    this.BuildingBlocksCompletionArray = new List<BuildingBlockData>(this.buildingBlocksData.Count);
                    foreach (var item in this.buildingBlocksData)
                    {

                        BuildingBlocksCompletionArray.Add(new BuildingBlockData() { Guid = item.Key, Value = item.Value.Value, CompletionDate = item.Value.CompletionDate });
                    }
                }
                
                sr.Serialize(s, this);
            }
           
        }
    }
}
