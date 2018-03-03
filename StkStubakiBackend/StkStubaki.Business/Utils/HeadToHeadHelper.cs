using StkStubaki.Business.BO;
using StkStubaki.Business.DTO;
using StkStubaki.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkStubaki.Business.Utils
{
    public static class HeadToHeadHelper
    {
        public static Dictionary<int, List<HeadToHeadInfoDTO>> GenerateHeadToHeadInfoDTO<T>(Dictionary<HeadToHeadKey, HeadToHeadInfo<T>> allInfos) where T: IComparable
        {
            Dictionary<int, List<HeadToHeadInfoDTO>> infosDict = new Dictionary<int, List<HeadToHeadInfoDTO>>();

            foreach(var headToHeadInfo in allInfos)
            {
                var headStatus = headToHeadInfo.Value.Info1.CompareTo(headToHeadInfo.Value.Info2);
                if (!infosDict.ContainsKey(headToHeadInfo.Key.Id1))
                {
                    infosDict.Add(headToHeadInfo.Key.Id1, new List<HeadToHeadInfoDTO>());
                }

                if (!infosDict.ContainsKey(headToHeadInfo.Key.Id2))
                {
                    infosDict.Add(headToHeadInfo.Key.Id2, new List<HeadToHeadInfoDTO>());
                }

                infosDict[headToHeadInfo.Key.Id1].Add(new HeadToHeadInfoDTO(headToHeadInfo.Key.Id2, (HeadToHeadStatusEnum)headStatus));
                infosDict[headToHeadInfo.Key.Id2].Add(new HeadToHeadInfoDTO(headToHeadInfo.Key.Id1, (HeadToHeadStatusEnum)(-1 * headStatus)));
            }

            return infosDict;
        }
    }
}
