namespace LFrameWork.Base.Random
{
    using System.Collections;
    using System.Collections.Generic;

    public static class RandomHelper
    {
        static int mSeed = 0;
        static System.Random fixedSeedClass;
        static System.Random randomSeedClass;


        public static void Init(int seed)
        {
            mSeed = seed;
            fixedSeedClass = new System.Random(mSeed);
        }

        /// <summary>
        /// 获取固定种子的随机数，每局进游戏初始化一次，左闭右开[min,max)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomRangeByFixedSeed(int min, int max)
        {
            if(fixedSeedClass==null)
            {
                fixedSeedClass = new System.Random(mSeed);
            }
            return fixedSeedClass.Next(min, max);
        }

        /// <summary>
        /// 获取随机种子的随机数，每次种子都不一样，左闭右开[min,max)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomRangeByRandomSeed(int min, int max)
        {
            randomSeedClass = new System.Random(System.Guid.NewGuid().GetHashCode());
            return randomSeedClass.Next(min, max);
        }
    }
}
