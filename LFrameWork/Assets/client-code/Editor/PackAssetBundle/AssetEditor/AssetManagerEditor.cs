using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using LFrameWork.AssetManagement;
using LFrameWork.EditorUtils;


namespace LFrameWork.AssetManagementEditor
{
    [CustomEditor(typeof(AssetManager))]
    public class AssetManagerEditor : Editor
    {
        private static readonly string ASSET_MANAGER_HEAD = "AssetManager:";

        private AssetManager assetManager = null;

        private bool frameInfoAutoUpdate = false;
        private AssetManager.AssetFrameInfo frameInfo;
        

        private readonly Dictionary<string, bool> foldoutDic = new Dictionary<string, bool>();
        public bool IsFoldout(string str, bool defaultFoldout = true)
        {
            bool isFoldout = true;
            if (foldoutDic.TryGetValue(str, out isFoldout))
            {
            }
            else
            {
                isFoldout = defaultFoldout;
                foldoutDic.Add(str, isFoldout);
            }

            return isFoldout;
        }
        public void SetFoldout(string str, bool isFoldout)
        {
            foldoutDic[str] = isFoldout;
        }




        void OnEnable()
        {
            assetManager = (AssetManager)target;
        }

        private void OnDisable()
        {
            assetManager = null;
        }

        public override void OnInspectorGUI()
        {
            if (assetManager == null)
            {
                return;
            }
            using (new GUIBeginVertical())
            {
                DrawAssetCache();

                //DrawAssetRefInfo();
                DrawAbLoaders();
            }




            /**
            //设置整个界面是以垂直方向来布局
            EditorGUILayout.BeginVertical();

            //空两行
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //绘制palyer的基本信息
            EditorGUILayout.LabelField("Base Info");
            player.id = EditorGUILayout.IntField("Player ID", player.id);
            player.playerName = EditorGUILayout.TextField("PlayerName", player.playerName);

            //空三行
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //绘制Player的背景故事
            EditorGUILayout.LabelField("Back Story");
            player.backStory = EditorGUILayout.TextArea(player.backStory, GUILayout.MinHeight(100));

            //空三行
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //使用滑块绘制 Player 生命值
            player.health = EditorGUILayout.Slider("Health", player.health, 0, 100);

            //根据生命值设置生命条的背景颜色
            if (player.health < 20)
            {
                GUI.color = Color.red;
            }
            else if (player.health > 80)
            {
                GUI.color = Color.green;
            }
            else
            {
                GUI.color = Color.gray;
            }

            //指定生命值的宽高
            Rect progressRect = GUILayoutUtility.GetRect(50, 50);

            //绘制生命条
            EditorGUI.ProgressBar(progressRect, player.health / 100.0f, "Health");

            //用此处理，以防上面的颜色变化会影响到下面的颜色变化
            GUI.color = Color.white;

            //空三行
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //使用滑块绘制伤害值
            player.damage = EditorGUILayout.Slider("Damage", player.damage, 0, 20);

            //根据伤害值的大小设置显示的类型和提示语
            if (player.damage < 10)
            {
                EditorGUILayout.HelpBox("伤害太低了吧！！", MessageType.Error);
            }
            else if (player.damage > 15)
            {
                EditorGUILayout.HelpBox("伤害有点高啊！！", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("伤害适中！！", MessageType.Info);
            }

            //空三行
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //设置内容折叠
            showWeapons = EditorGUILayout.Foldout(showWeapons, "Weapons");
            if (showWeapons)
            {
                player.weaponDamage1 = EditorGUILayout.FloatField("Weapon 1 Damage", player.weaponDamage1);
                player.weaponDamage2 = EditorGUILayout.FloatField("Weapon 2 Damage", player.weaponDamage2);
            }

            //空三行
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //绘制鞋子信息
            EditorGUILayout.LabelField("Shoe");
            //以水平方向绘制
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.MaxWidth(50));
            player.shoeName = EditorGUILayout.TextField(player.shoeName);
            EditorGUILayout.LabelField("Size", GUILayout.MaxWidth(50));
            player.shoeSize = EditorGUILayout.IntField(player.shoeSize);
            EditorGUILayout.LabelField("Type", GUILayout.MaxWidth(50));
            player.shoeType = EditorGUILayout.TextField(player.shoeType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            //*/
        }

        private void DrawAssetRefList(AssetRefList refList, string name)
        {
            using (new GUIBeginVertical())
            {
                string foldName = "DrawAssetRefList " + name;
                bool foldout = EditorGUILayout.Foldout(IsFoldout(foldName, false), string.Format("{0} : {1}", name, refList.Count));
                SetFoldout(foldName, foldout);

                if (foldout)
                {
                    using (new GUIIndentLevel())
                    {
                        for (int i = 0; i < refList.Count; ++i)
                        {
                            AssetRef assetRef = refList.GetAt(i);
                            if (assetRef == null || assetRef.asset == null)
                            {
                                continue;
                            }
                            using (new GUIBeginVertical())
                            {
                                //using (new GUIBeginHorizontal())
                                {
                                    EditorGUILayout.ObjectField(assetRef.asset, assetRef.asset.GetType(), false, null);
                                //    EditorGUILayout.LabelField(string.Format("ref:{0}", assetRef.refCount));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawAssetCache()
        {
            using (new GUIBeginVertical())
            {
                string foldName = "AssetCache";
                bool foldout = EditorGUILayout.Foldout(IsFoldout(foldName), foldName);
                SetFoldout(foldName, foldout);

                if (foldout)
                {
                    using (new GUIIndentLevel())
                    {
                        DrawAssetRefList(assetManager.StrongRefs, "StrongRefs");
                        DrawAssetRefList(assetManager.WeakRefs, "WeakRefs");
                    }
                }
            }
        }

        private void DrawAssetRefInfo()
        {
            using (new GUIBeginVertical())
            {
                string foldName = "AssetRef";
                bool foldout = EditorGUILayout.Foldout(IsFoldout(foldName), foldName);
                SetFoldout(foldName, foldout);

                if (foldout)
                {
                    using (new GUIIndentLevel())
                    {
                        frameInfoAutoUpdate = EditorGUILayout.Toggle("AutoUpdate", frameInfoAutoUpdate);
                        if (frameInfoAutoUpdate)
                        {
                            frameInfo = assetManager.CaptureFrame();
                        }

                        EditorGUILayout.LabelField(string.Format("HitCache = {0}", frameInfo.cacheHitPercent));
                        if (frameInfo.assetInfos != null)
                        {
                            if ( GUIFoldout.Foldout(ASSET_MANAGER_HEAD + " AssetCount", string.Format("AssetCount = {0}", frameInfo.assetInfos.Length)) )
                            {
                                for (int i = 0; i < frameInfo.assetInfos.Length; ++i)
                                {
                                    AssetManager.AssetInfo assetInfo = frameInfo.assetInfos[i];
                                    EditorGUILayout.ObjectField(assetInfo.asset, assetInfo.asset.GetType(), false, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawAbLoaders()
        {
            if ( GUIFoldout.Foldout(ASSET_MANAGER_HEAD + " AbLoaders", "AbLoaders" ) )
            {
                using (new GUIIndentLevel())
                {
                    if (assetManager != null && assetManager.AbLoaders != null)
                    {
                        for (int i = 0; i < assetManager.AbLoaders.Count; ++i)
                        {
                            var abLoader = assetManager.AbLoaders[i];
                            if (abLoader != null && abLoader.RefManager != null)
                            {
                                DrawRefManager(abLoader.RefManager);
                                DrawRefManager(abLoader.BundleRefMgr);
                                DrawLruInfo(abLoader);
                            }
                        }
                    }
                }
            }
        }

        private void DrawRefManager(AssetRefManager refManager)
        {
            string foldoutContent = string.Format("{0}:{1}", refManager.name, refManager.AssetRefMap.Count);
            if (GUIFoldout.Foldout(ASSET_MANAGER_HEAD + " RefManager" + refManager.name, foldoutContent, false))
            {
                using (new GUIBeginVertical())
                {
                    for (var e = refManager.AssetRefMap.GetEnumerator(); e.MoveNext();)
                    {
                        AssetRef assetRef = e.Current.Value;
                        EditorGUILayout.ObjectField(assetRef.asset, assetRef.asset.GetType(), false, null);
                    }
                }
            }
        }

        private void DrawLruInfo(AbLoader abLoader)
        {
            string[] lru = abLoader.GetLruInfo();
            if (lru == null )
            {
                return;
            }
            string foldoutContent = string.Format("lruCache:{0}", lru.Length);
            string fdName = ASSET_MANAGER_HEAD + " " + abLoader.LoaderName + "_lru";
            if (GUIFoldout.Foldout(fdName, foldoutContent, false))
            {
                using ( new GUIIndentLevel() )
                {
                    for (int i = 0; i < lru.Length; ++i)
                    {
                        EditorGUILayout.TextField(lru[i]);
                    }
                }
            }
        }
    }
}