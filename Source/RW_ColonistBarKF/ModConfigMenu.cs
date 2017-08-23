﻿using static ColonistBarKF.PSI.GameComponentPSI;

namespace ColonistBarKF
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using ColonistBarKF.Bar;

    using FacialStuff.Detouring;

    using JetBrains.Annotations;

    using static Settings;

    using UnityEngine;

    using Verse;

    public class ColonistBarKfSettings : Window
    {

        #region Public Fields

        public static int Lastupdate = -5000;

        // private static ColorWrapper colourWrapper;
        public Window OptionsDialog;

        #endregion Public Fields

        #region Private Fields

        private static readonly string Cbkfversion = "Colonist Bar KF 0.17.3.1";

        private static int iconLimit;

        [NotNull]
        private readonly GUIStyle darkGrayBgImage =
            new GUIStyle { normal = { background = ColonistBarTextures.GrayFond } };

        [NotNull]
        private readonly GUIStyle fondBoxes =
            new GUIStyle
            {
                normal = {
                                background = ColonistBarTextures.DarkGrayFond
                             },
                hover = {
                               background = ColonistBarTextures.GrayFond
                            },
                padding = new RectOffset(15, 15, 6, 10),
                margin = new RectOffset(0, 0, 10, 10)
            };

        [NotNull]
        private readonly GUIStyle fondImages =
            new GUIStyle
            {
                normal = {
                                background = ColonistBarTextures.DarkGrayFond
                             },
                hover = {
                               background = ColonistBarTextures.RedHover
                            }
            };

        [NotNull]
        private readonly GUIStyle fontBold =
            new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal = {
                                textColor = Color.white
                             },
                padding = new RectOffset(0, 0, 5, 0)
            };

        [NotNull]
        private readonly GUIStyle grayLines = new GUIStyle { normal = { background = ColonistBarTextures.GrayLines } };

        [NotNull]
        private readonly GUIStyle headline =
            new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 16,
                normal = {
                                textColor = Color.white
                             },
                padding = new RectOffset(0, 0, 12, 6)
            };

        [NotNull]
        private readonly string[] mainToolbarStrings =
            {
                "CBKF.Settings.ColonistBar".Translate(), "CBKF.Settings.PSI".Translate()

            };

        [NotNull]
        private readonly string[] positionStrings =
            {
                "CBKF.Settings.useLeft".Translate(), "CBKF.Settings.useRight".Translate(),
                "CBKF.Settings.useTop".Translate(), "CBKF.Settings.useBottom".Translate()
            };

        [NotNull]
        private readonly string[] psiToolbarStrings =
            {
                "PSI.Settings.ArrangementButton".Translate(), "PSI.Settings.OpacityButton".Translate(),
                "PSI.Settings.IconButton".Translate(), "PSI.Settings.SensitivityButton".Translate()
            };

        private int moodBarPositionInt;

        private int psiBarPositionInt;

        private int psiPositionInt;

        private Vector2 scrollPositionBase;

        private Vector2 scrollPositionPSI;

        private Vector2 scrollPositionPSIOp;

        private Vector2 scrollPositionPSISens;

        private Vector2 scrollPositionPSISize;

        #endregion Private Fields

        #region Public Constructors

        public ColonistBarKfSettings()
        {
            this.forcePause = false;
            this.doCloseX = true;
            this.draggable = true;
            this.drawShadow = true;
            this.preventCameraMotion = false;
            this.resizeable = true;
            this.onlyOneOfTypeAllowed = true;
            Reinit(false, true);
        }

        #endregion Public Constructors

        #region Public Properties

        public override Vector2 InitialSize => new Vector2(540f, 650f);

        #endregion Public Properties

        #region Private Properties

        private int MainToolbarInt { get; set; }

        private int MoodBarPositionInt
        {
            get
            {
                switch (ColBarSettings.MoodBarPos)
                {
                    case Position.Alignment.Left:
                        this.moodBarPositionInt = 0;
                        break;

                    case Position.Alignment.Right:
                        this.moodBarPositionInt = 1;
                        break;

                    case Position.Alignment.Top:
                        this.moodBarPositionInt = 2;
                        break;

                    case Position.Alignment.Bottom:
                        this.moodBarPositionInt = 3;
                        break;
                }

                return this.moodBarPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        ColBarSettings.MoodBarPos = Position.Alignment.Left;
                        break;

                    case 1:
                        ColBarSettings.MoodBarPos = Position.Alignment.Right;
                        break;

                    case 2:
                        ColBarSettings.MoodBarPos = Position.Alignment.Top;
                        break;

                    case 3:
                        ColBarSettings.MoodBarPos = Position.Alignment.Bottom;
                        break;

                    default:
                        ColBarSettings.MoodBarPos = Position.Alignment.Right;
                        break;
                }

                this.moodBarPositionInt = value;
            }
        }

        private int PsiBarPositionInt
        {
            get
            {
                if (ColBarSettings.ColBarPsiIconPos == Position.Alignment.Left)
                {
                    this.psiBarPositionInt = 0;
                }

                if (ColBarSettings.ColBarPsiIconPos == Position.Alignment.Right)
                {
                    this.psiBarPositionInt = 1;
                }

                if (ColBarSettings.ColBarPsiIconPos == Position.Alignment.Top)
                {
                    this.psiBarPositionInt = 2;
                }

                if (ColBarSettings.ColBarPsiIconPos == Position.Alignment.Bottom)
                {
                    this.psiBarPositionInt = 3;
                }

                return this.psiBarPositionInt;
            }

            set
            {
                switch (value)
                {
                    case 0:
                        ColBarSettings.ColBarPsiIconPos = Position.Alignment.Left;
                        ColBarSettings.IconOffsetX = 1f;
                        ColBarSettings.IconOffsetY = 1f;
                        ColBarSettings.IconsHorizontal = false;
                        break;

                    case 1:
                        ColBarSettings.ColBarPsiIconPos = Position.Alignment.Right;
                        ColBarSettings.IconOffsetX = -1f;
                        ColBarSettings.IconOffsetY = 1f;
                        ColBarSettings.IconsHorizontal = false;
                        break;

                    case 2:
                        ColBarSettings.ColBarPsiIconPos = Position.Alignment.Top;
                        ColBarSettings.IconOffsetX = -1f;
                        ColBarSettings.IconOffsetY = 1f;
                        ColBarSettings.IconsHorizontal = true;
                        break;

                    case 3:
                        ColBarSettings.ColBarPsiIconPos = Position.Alignment.Bottom;
                        ColBarSettings.IconOffsetX = -1;
                        ColBarSettings.IconOffsetY = -1;
                        ColBarSettings.IconsHorizontal = true;
                        break;

                    default:
                        ColBarSettings.ColBarPsiIconPos = 0;

                        break;
                }

                this.psiBarPositionInt = value;
            }
        }

        private int PsiPositionInt
        {
            get
            {
                if (PsiSettings.IconAlignment == 0)
                {
                    this.psiPositionInt = 0;
                }

                if (PsiSettings.IconAlignment == 1)
                {
                    this.psiPositionInt = 1;
                }

                if (PsiSettings.IconAlignment == 2)
                {
                    this.psiPositionInt = 2;
                }

                if (PsiSettings.IconAlignment == 3)
                {
                    this.psiPositionInt = 3;
                }

                return this.psiPositionInt;
            }

            set
            {
                if (value == this.psiPositionInt)
                {
                    return;
                }

                switch (value)
                {
                    case 0:
                        PsiSettings.IconAlignment = value;
                        PsiSettings.IconMarginX = 1f;
                        PsiSettings.IconMarginY = 1f;
                        PsiSettings.IconOffsetX = 1f;
                        PsiSettings.IconOffsetY = 1f;
                        PsiSettings.IconsHorizontal = false;
                        PsiSettings.IconsScreenScale = true;
                        PsiSettings.IconsInColumn = 3;
                        PsiSettings.IconSize = 1f;
                        PsiSettings.IconOpacity = 0.5f;
                        PsiSettings.IconOpacityCritical = 0.8f;
                        break;

                    case 1:
                        PsiSettings.IconAlignment = value;
                        PsiSettings.IconMarginX = -1f;
                        PsiSettings.IconMarginY = 1f;
                        PsiSettings.IconOffsetX = -1f;
                        PsiSettings.IconOffsetY = 1f;
                        PsiSettings.IconsHorizontal = false;
                        PsiSettings.IconsScreenScale = true;
                        PsiSettings.IconsInColumn = 3;
                        PsiSettings.IconSize = 1f;
                        PsiSettings.IconOpacity = 0.5f;
                        PsiSettings.IconOpacityCritical = 0.8f;
                        break;

                    case 2:
                        PsiSettings.IconAlignment = value;
                        PsiSettings.IconMarginX = 1f;
                        PsiSettings.IconMarginY = -1.63f;
                        PsiSettings.IconOffsetX = -1f;
                        PsiSettings.IconOffsetY = 1f;
                        PsiSettings.IconsHorizontal = true;
                        PsiSettings.IconsScreenScale = true;
                        PsiSettings.IconsInColumn = 3;
                        PsiSettings.IconSize = 1f;
                        PsiSettings.IconOpacity = 0.5f;
                        PsiSettings.IconOpacityCritical = 0.8f;
                        break;

                    case 3:
                        PsiSettings.IconAlignment = value;
                        PsiSettings.IconMarginX = 1.139534f;
                        PsiSettings.IconMarginY = 1.375f;
                        PsiSettings.IconOffsetX = -0.9534883f;
                        PsiSettings.IconOffsetY = -0.9534884f;
                        PsiSettings.IconsHorizontal = true;
                        PsiSettings.IconsScreenScale = true;
                        PsiSettings.IconsInColumn = 4;
                        PsiSettings.IconSize = 1.084302f;
                        PsiSettings.IconOpacity = 0.5f;
                        PsiSettings.IconOpacityCritical = 0.8f;
                        break;

                    default:
                        PsiSettings.IconAlignment = 0;

                        break;
                }

                this.psiPositionInt = value;
            }
        }

        private int PSIToolbarInt { get; set; }

        #endregion Private Properties

        #region Public Methods

        public override void DoWindowContents(Rect rect)
        {
            Rect viewRect = new Rect(rect);
            viewRect.x += 15f;
            viewRect.width -= 30f;
            viewRect.height -= 15f;

            GUILayout.BeginArea(viewRect);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(Cbkfversion, this.headline);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(Text.LineHeight / 2);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            this.MainToolbarInt = GUILayout.Toolbar(this.MainToolbarInt, this.mainToolbarStrings);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            switch (this.MainToolbarInt)
            {
                case 0:
                    {
                        GUILayout.Space(Text.LineHeight / 2);

                        this.scrollPositionBase = GUILayout.BeginScrollView(this.scrollPositionBase);

                        this.LabelHeadline("CBKF.Settings.BarPosition".Translate());
                        GUILayout.BeginVertical();
                        this.FillPageMain();
                        GUILayout.EndVertical();

                        this.LabelHeadline("CBKF.Settings.Advanced".Translate());
                        GUILayout.BeginVertical();
                        this.FillPageAdvanced();
                        GUILayout.EndVertical();

                        this.LabelHeadline("CBKF.Settings.Grouping".Translate());
                        GUILayout.BeginVertical();
                        this.FillPageCaravanSettings();
                        GUILayout.EndVertical();


                        GUILayout.EndScrollView();
                    }

                    break;

                case 1:
                    {
                       // LabelHeadline("PSI.Settings.Title".Translate());
                        GUILayout.Space(Text.LineHeight);

                        int toolbarInt = Mathf.FloorToInt(viewRect.width / 150f);
                        if (toolbarInt == 0)
                        {
                            toolbarInt += 1;
                        }

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        this.PSIToolbarInt = GUILayout.SelectionGrid(
                            this.PSIToolbarInt,
                            this.psiToolbarStrings,
                            toolbarInt > this.psiToolbarStrings.Length ? this.psiToolbarStrings.Length : toolbarInt);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.Space(Text.LineHeight / 2);

                        if (this.PSIToolbarInt == 0)
                        {
                            {
                                this.FillPSIPageSizeArrangement();
                            }
                        }
                        else if (this.PSIToolbarInt == 1)
                        {
                            {
                                this.FillPagePSIOpacityAndColor();
                            }
                        }
                        else if (this.PSIToolbarInt == 2)
                        {
                            {
                                this.FillPagePSIIconSet(viewRect);
                            }
                        }
                        else if (this.PSIToolbarInt == 3)
                        {
                            {
                                this.FillPSIPageSensitivity();
                            }
                        }

                        // else if (PSIToolbarInt == 4)
                        // {
                        // {
                        // FillPagePSILoadIconset();
                        // }
                        // }
                        else
                        {
                            this.FillPagePSIIconSet(viewRect);
                        }
                    }

                    break;
            }

            GUILayout.FlexibleSpace();
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.Label(string.Empty, this.grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("CBKF.Settings.RevertSettings".Translate()))
            {
                this.ResetBarSettings();
            }

            GUILayout.Space(Text.LineHeight / 2);
            if (GUILayout.Button("PSI.Settings.RevertSettings".Translate()))
            {
                this.ResetPSISettings();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();

            if (GUI.changed)
            {
                HarmonyPatches.MarkColonistsDirty_Postfix();
                Reinit(false, false, true);
            }
        }

        public override void PreClose()
        {
            SaveBarSettings();
            SavePsiSettings();
        }

        #endregion Public Methods

        #region Private Methods

        private void DrawCheckboxArea(
            [NotNull] string iconName,
            [NotNull] Material iconMaterial,
            ref bool colBarBool,
            ref bool psiBarBool,
            ref int iconInRow)
        {
            if (iconInRow > iconLimit - 1)
            {
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginHorizontal();
                iconInRow = 0;
            }

            if (iconInRow > 0 && iconLimit != 1)
            {
                GUILayout.Space(Text.LineHeight / 2);
            }

            GUILayout.BeginVertical(this.fondImages);

            GUILayout.BeginHorizontal(this.darkGrayBgImage, GUILayout.Height(Text.LineHeight * 1.2f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(iconName, this.fontBold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Space(Text.LineHeight / 2);

            if (iconMaterial != null)
            {
                GUILayout.Label(
                    iconMaterial.mainTexture,
                    GUILayout.Width(Text.LineHeight * 2.5f),
                    GUILayout.Height(Text.LineHeight * 2.5f));
            }
            else
            {
                if (psiBarBool || colBarBool)
                {
                    GUI.color = Color.red;
                    GUILayout.Label(
                        "PSI.Settings.IconSet.IconNotAvailable".Translate(),
                        GUILayout.Width(Text.LineHeight * 2.5f),
                        GUILayout.Height(Text.LineHeight * 2.5f));
                    GUI.color = Color.white;
                }
                else
                {
                    GUILayout.Label(
                        string.Empty,
                        GUILayout.Width(Text.LineHeight * 2.5f),
                        GUILayout.Height(Text.LineHeight * 2.5f));
                }
            }

            GUILayout.Space(Text.LineHeight / 2);

            // Label("PSI.Settings.VisibilityButton".Translate(), FontBold);
            colBarBool = GUILayout.Toggle(colBarBool, "CBKF.Settings.ColBarIconVisibility".Translate());
            psiBarBool = GUILayout.Toggle(psiBarBool, "CBKF.Settings.PSIIconVisibility".Translate());
            GUILayout.Space(Text.LineHeight / 2);

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            iconInRow++;
        }

        private void DrawCheckboxAreaTarget(
            [NotNull] string iconName,
            [NotNull] Material targetSingle,
            [NotNull] Material targetHair,
            [NotNull] Material targetSkin,
            ref int iconInRow)
        {
            GUILayout.BeginVertical(this.fondImages);

            GUILayout.BeginHorizontal(this.darkGrayBgImage, GUILayout.Height(Text.LineHeight * 1.2f));
            GUILayout.FlexibleSpace();
            GUILayout.Label(iconName, this.fontBold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Space(Text.LineHeight / 2);
            if (PsiSettings.ShowTargetPoint)
            {
                if (!PsiSettings.UseColoredTarget)
                {
                    if (targetSingle != null)
                    {
                        GUILayout.Label(
                            targetSingle.mainTexture,
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label(
                            "PSI.Settings.IconSet.IconNotAvailable".Translate(),
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    if (targetHair != null)
                    {
                        GUILayout.Label(
                            targetHair.mainTexture,
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label(
                            "PSI.Settings.IconSet.IconNotAvailable".Translate() + " HairTarget",
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }

                    if (targetSkin != null)
                    {
                        GUILayout.Label(
                            targetSkin.mainTexture,
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label(
                            "PSI.Settings.IconSet.IconNotAvailable".Translate() + " SkinTarget",
                            GUILayout.Width(Text.LineHeight * 2.5f),
                            GUILayout.Height(Text.LineHeight * 2.5f));
                        GUI.color = Color.white;
                    }

                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label(
                    string.Empty,
                    GUILayout.Width(Text.LineHeight * 2.5f),
                    GUILayout.Height(Text.LineHeight * 2.5f));
            }

            GUILayout.Space(Text.LineHeight / 2);

            // Label("PSI.Settings.VisibilityButton".Translate(), FontBold);
            PsiSettings.ShowTargetPoint = GUILayout.Toggle(
                PsiSettings.ShowTargetPoint,
                "PSI.Settings.Visibility.TargetPoint".Translate());
            PsiSettings.UseColoredTarget = GUILayout.Toggle(
                PsiSettings.UseColoredTarget,
                "PSI.Settings.IconOpacityAndColor.UseColoredTarget".Translate());

            GUILayout.Space(Text.LineHeight / 2);

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            iconInRow += 2;
        }

        private void FillPageAdvanced()
        {
            GUILayout.BeginVertical(this.fondBoxes);

            ColBarSettings.UseCustomIconSize = GUILayout.Toggle(
                ColBarSettings.UseCustomIconSize,
                "CBKF.Settings.BasicSize".Translate() + ColBarSettings.BaseSizeFloat.ToString("N0") + " px, "
                + ColonistBar_KF.BarHelperKf.cachedScale.ToStringPercent() + " %, "
                + (int)ColBarSettings.BaseSpacingHorizontal + " x, " + (int)ColBarSettings.BaseSpacingVertical + " y");

            if (ColBarSettings.UseCustomIconSize)
            {
                GUILayout.Space(Text.LineHeight / 2);

                ColBarSettings.BaseSizeFloat = GUILayout.HorizontalSlider(ColBarSettings.BaseSizeFloat, 24f, 256f);

                ColBarSettings.BaseSpacingHorizontal =
                    GUILayout.HorizontalSlider(ColBarSettings.BaseSpacingHorizontal, 1f, 72f);
                ColBarSettings.BaseSpacingVertical =
                    GUILayout.HorizontalSlider(ColBarSettings.BaseSpacingVertical, 1f, 96f);
            }
            else
            {
                ColBarSettings.BaseSizeFloat = 48f;
                ColBarSettings.BaseSpacingHorizontal = 24f;
                ColBarSettings.BaseSpacingVertical = 32f;
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            ColBarSettings.UseCustomPawnTextureCameraOffsets = GUILayout.Toggle(
                ColBarSettings.UseCustomPawnTextureCameraOffsets,
                "CBKF.Settings.PawnTextureCameraOffsets".Translate()
                + ColBarSettings.PawnTextureCameraHorizontalOffset.ToString("N2") + " x, "
                + ColBarSettings.PawnTextureCameraVerticalOffset.ToString("N2") + " y, "
                + ColBarSettings.PawnTextureCameraZoom.ToString("N2") + " z");
            if (ColBarSettings.UseCustomPawnTextureCameraOffsets)
            {
                GUILayout.Space(Text.LineHeight / 2);
                ColBarSettings.PawnTextureCameraHorizontalOffset = GUILayout.HorizontalSlider(
                    ColBarSettings.PawnTextureCameraHorizontalOffset,
                    0.7f,
                    -0.7f);
                ColBarSettings.PawnTextureCameraVerticalOffset =
                    GUILayout.HorizontalSlider(ColBarSettings.PawnTextureCameraVerticalOffset, 0f, 1f);
                ColBarSettings.PawnTextureCameraZoom =
                    GUILayout.HorizontalSlider(ColBarSettings.PawnTextureCameraZoom, 0.3f, 3f);
            }
            else
            {
                ColBarSettings.PawnTextureCameraHorizontalOffset = 0f;
                ColBarSettings.PawnTextureCameraVerticalOffset = 0.3f;
                ColBarSettings.PawnTextureCameraZoom = 1.28205f;
            }

            GUILayout.EndVertical();

            // if (ColBarSettings.UseGender)
            // {
            // if (Button("CBKF.Settings.FemaleColor".Translate()))
            // {
            // while (Find.WindowStack.TryRemove(typeof(Dialog_ColorPicker)))
            // {
            // }
            // Find.WindowStack.Add(new Dialog_ColorPicker(colourWrapper, delegate
            // {
            // ColBarSettings.FemaleColor = colourWrapper.Color;
            // }, false, true)
            // {
            // initialPosition = new Vector2(windowRect.xMax + 10f, windowRect.yMin),
            // });
            // }
            // if (Button("CBKF.Settings.MaleColor".Translate()))
            // {
            // while (Find.WindowStack.TryRemove(typeof(Dialog_ColorPicker)))
            // {
            // }
            // Find.WindowStack.Add(new Dialog_ColorPicker(colourWrapper, delegate
            // {
            // ColBarSettings.MaleColor = colourWrapper.Color;
            // }, false, true)
            // {
            // initialPosition = new Vector2(windowRect.xMax + 10f, windowRect.yMin),
            // });
            // }
            // if (Button("CBKF.Settings.ResetColors".Translate()))
            // {
            // ColBarSettings.FemaleColor = new Color(1f, 0.64f, 0.8f, 1f);
            // ColBarSettings.MaleColor = new Color(0.52f, 0.75f, 0.92f, 1f);
            // }
            // }
        }

        private void FillPageCaravanSettings()
        {
            GUILayout.BeginVertical(this.fondBoxes);
            ColBarSettings.UseGrouping = GUILayout.Toggle(
                ColBarSettings.UseGrouping,
                "CBKF.Settings.UseGrouping".Translate());

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            ColBarSettings.UseGroupColors = GUILayout.Toggle(
                ColBarSettings.UseGroupColors,
                "CBKF.Settings.UseGroupColors".Translate());

            GUILayout.EndVertical();
        }

        private void FillPageMain()
        {
            GUILayout.BeginVertical(this.fondBoxes);
            ColBarSettings.UseCustomMarginTop = GUILayout.Toggle(
                ColBarSettings.UseCustomMarginTop,
                "CBKF.Settings.ColonistBarOffset".Translate() + (int)ColBarSettings.MarginTop + " y \n"
                + "CBKF.Settings.MaxColonistBarWidth".Translate() + ": "
                + ((float)UI.screenWidth - (int)ColBarSettings.MarginHorizontal) + " px");

            if (ColBarSettings.UseCustomMarginTop)
            {
                GUILayout.Space(Text.LineHeight / 2);
                ColBarSettings.MarginTop =
                    GUILayout.HorizontalSlider(ColBarSettings.MarginTop, 0f, (float)UI.screenHeight / 6);
                ColBarSettings.MarginHorizontal = GUILayout.HorizontalSlider(
                    ColBarSettings.MarginHorizontal,
                    (float)UI.screenWidth * 3 / 5,
                    0f);
            }
            else
            {
                ColBarSettings.MarginTop = 21f;
                ColBarSettings.MarginHorizontal = 520f;
            }

            // listing.Gap(3f);
            GUILayout.EndVertical();

            // listing.Gap(3f);
            GUILayout.BeginVertical(this.fondBoxes);
            ColBarSettings.UseCustomRowCount = GUILayout.Toggle(
                ColBarSettings.UseCustomRowCount,
                "PSI.Settings.Arrangement.ColonistsPerColumn".Translate()
                + (ColBarSettings.UseCustomRowCount ? ColBarSettings.MaxRowsCustom : 3));
            if (ColBarSettings.UseCustomRowCount)
            {
                ColBarSettings.MaxRowsCustom = (int)GUILayout.HorizontalSlider(ColBarSettings.MaxRowsCustom, 1f, 5f);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);

            ColBarSettings.UseWeaponIcons = GUILayout.Toggle(
                ColBarSettings.UseWeaponIcons,
                "CBKF.Settings.UseWeaponIcons".Translate());

            ColBarSettings.UseGender = GUILayout.Toggle(
                ColBarSettings.UseGender,
                "CBKF.Settings.useGender".Translate());

            ColBarSettings.useZoomToMouse = GUILayout.Toggle(
                ColBarSettings.useZoomToMouse,
                "CBKF.Settings.useZoomToMouse".Translate());

            GUILayout.Label("FollowMe.MiddleClick".Translate());

            // #region DoubleClickTime
            // ColBarSettings.UseCustomDoubleClickTime = GUILayout.Toggle(
            // ColBarSettings.UseCustomDoubleClickTime,
            // "CBKF.Settings.DoubleClickTime".Translate() + ": " + ColBarSettings.DoubleClickTime.ToString("N2")
            // + " s");
            // if (ColBarSettings.UseCustomDoubleClickTime)
            // {
            // // listing.Gap(3f);
            // GUILayout.Space(Text.LineHeight / 2);
            // ColBarSettings.DoubleClickTime = GUILayout.HorizontalSlider(ColBarSettings.DoubleClickTime, 0.1f, 1.5f);
            // }
            // else
            // {
            // ColBarSettings.DoubleClickTime = 0.5f;
            // }
            // #endregion
            GUILayout.BeginVertical(this.fondBoxes);
            ColBarSettings.UseNewMood = GUILayout.Toggle(
                ColBarSettings.UseNewMood,
                "CBKF.Settings.UseNewMood".Translate());

            if (ColBarSettings.UseNewMood)
            {
                ColBarSettings.UseExternalMoodBar = GUILayout.Toggle(
                    ColBarSettings.UseExternalMoodBar,
                    "CBKF.Settings.UseExternalMoodBar".Translate());

                if (ColBarSettings.UseExternalMoodBar)
                {
                    GUILayout.BeginHorizontal();
                    this.MoodBarPositionInt = GUILayout.Toolbar(this.MoodBarPositionInt, this.positionStrings);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                ColBarSettings.UseExternalMoodBar = false;
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

        private void FillPagePSIIconSet(Rect viewRect)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PSI.Settings.IconSet".Translate() + PsiSettings.IconSet))
            {
                FloatMenuOption fmoDefault = new FloatMenuOption(
                    "PSI.Settings.Preset.0".Translate(),
                    () =>
                        {
                            try
                            {
                                PsiSettings.IconSet = "default";
                                PsiSettings.UseColoredTarget = true;
                                SavePsiSettings();
                                Reinit(false, true, false);
                            }
                            catch (IOException)
                            {
                                Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "default");
                            }
                        });

                FloatMenuOption fmoPreset1 = new FloatMenuOption(
                    "PSI.Settings.Preset.1".Translate(),
                    () =>
                        {
                            try
                            {
                                PsiSettings.IconSet = "original";
                                PsiSettings.UseColoredTarget = false;
                                SavePsiSettings();
                                Reinit(false, true, false);
                            }
                            catch (IOException)
                            {
                                Log.Error("PSI.Settings.LoadPreset.UnableToLoad".Translate() + "default");
                            }
                        });

                List<FloatMenuOption> options = new List<FloatMenuOption> { fmoDefault, fmoPreset1 };

                Find.WindowStack.Add(new FloatMenu(options));
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            iconLimit = Mathf.FloorToInt(viewRect.width / 125f);

            GUILayout.Space(Text.LineHeight / 2);

            this.scrollPositionPSI = GUILayout.BeginScrollView(this.scrollPositionPSI);

            int num = 0;
            GUILayout.BeginHorizontal();

            this.DrawCheckboxAreaTarget(
                "PSI.Settings.Visibility.TargetPoint".Translate(),
                PSIMaterials[Icon.Target],
                PSIMaterials[Icon.TargetHair],
                PSIMaterials[Icon.TargetSkin],
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Draft".Translate(),
                PSIMaterials[Icon.Draft],
                ref ColBarSettings.ShowDraft,
                ref PsiSettings.ShowDraft,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Unarmed".Translate(),
                PSIMaterials[Icon.Unarmed],
                ref ColBarSettings.ShowUnarmed,
                ref PsiSettings.ShowUnarmed,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Idle".Translate(),
                PSIMaterials[Icon.Idle],
                ref ColBarSettings.ShowIdle,
                ref PsiSettings.ShowIdle,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Sad".Translate(),
                PSIMaterials[Icon.Sad],
                ref ColBarSettings.ShowSad,
                ref PsiSettings.ShowSad,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Aggressive".Translate(),
                PSIMaterials[Icon.Aggressive],
                ref ColBarSettings.ShowAggressive,
                ref PsiSettings.ShowAggressive,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Panic".Translate(),
                PSIMaterials[Icon.Panic],
                ref ColBarSettings.ShowPanic,
                ref PsiSettings.ShowPanic,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Dazed".Translate(),
                PSIMaterials[Icon.Dazed],
                ref ColBarSettings.ShowDazed,
                ref PsiSettings.ShowDazed,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Leave".Translate(),
                PSIMaterials[Icon.Leave],
                ref ColBarSettings.ShowLeave,
                ref PsiSettings.ShowLeave,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Hungry".Translate(),
                PSIMaterials[Icon.Hungry],
                ref ColBarSettings.ShowHungry,
                ref PsiSettings.ShowHungry,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Tired".Translate(),
                PSIMaterials[Icon.Tired],
                ref ColBarSettings.ShowTired,
                ref PsiSettings.ShowTired,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.TooCold".Translate(),
                PSIMaterials[Icon.TooCold],
                ref ColBarSettings.ShowTooCold,
                ref PsiSettings.ShowTooCold,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.TooHot".Translate(),
                PSIMaterials[Icon.TooHot],
                ref ColBarSettings.ShowTooHot,
                ref PsiSettings.ShowTooHot,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.ApparelHealth".Translate(),
                PSIMaterials[Icon.ApparelHealth],
                ref ColBarSettings.ShowApparelHealth,
                ref PsiSettings.ShowApparelHealth,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Naked".Translate(),
                PSIMaterials[Icon.Naked],
                ref ColBarSettings.ShowNaked,
                ref PsiSettings.ShowNaked,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Health".Translate(),
                PSIMaterials[Icon.Health],
                ref ColBarSettings.ShowHealth,
                ref PsiSettings.ShowHealth,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.MedicalAttention".Translate(),
                PSIMaterials[Icon.MedicalAttention],
                ref ColBarSettings.ShowMedicalAttention,
                ref PsiSettings.ShowMedicalAttention,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Injury".Translate(),
                PSIMaterials[Icon.Effectiveness],
                ref ColBarSettings.ShowEffectiveness,
                ref PsiSettings.ShowEffectiveness,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Bloodloss".Translate(),
                PSIMaterials[Icon.Bloodloss],
                ref ColBarSettings.ShowBloodloss,
                ref PsiSettings.ShowBloodloss,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pain".Translate(),
                PSIMaterials[Icon.Pain],
                ref ColBarSettings.ShowPain,
                ref PsiSettings.ShowPain,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Drunk".Translate(),
                PSIMaterials[Icon.Drunk],
                ref ColBarSettings.ShowDrunk,
                ref PsiSettings.ShowDrunk,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Toxicity".Translate(),
                PSIMaterials[Icon.Toxicity],
                ref ColBarSettings.ShowToxicity,
                ref PsiSettings.ShowToxicity,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.NightOwl".Translate(),
                PSIMaterials[Icon.NightOwl],
                ref ColBarSettings.ShowNightOwl,
                ref PsiSettings.ShowNightOwl,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.LeftUnburied".Translate(),
                PSIMaterials[Icon.LeftUnburied],
                ref ColBarSettings.ShowLeftUnburied,
                ref PsiSettings.ShowLeftUnburied,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.CabinFever".Translate(),
                PSIMaterials[Icon.CabinFever],
                ref ColBarSettings.ShowCabinFever,
                ref PsiSettings.ShowCabinFever,
                ref num);

            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Bedroom".Translate(),
                PSIMaterials[Icon.Bedroom],
                ref ColBarSettings.ShowBedroom,
                ref PsiSettings.ShowBedroom,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Greedy".Translate(),
                PSIMaterials[Icon.Greedy],
                ref ColBarSettings.ShowGreedy,
                ref PsiSettings.ShowGreedy,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Jealous".Translate(),
                PSIMaterials[Icon.Jealous],
                ref ColBarSettings.ShowJealous,
                ref PsiSettings.ShowJealous,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pyromaniac".Translate(),
                PSIMaterials[Icon.Pyromaniac],
                ref ColBarSettings.ShowPyromaniac,
                ref PsiSettings.ShowPyromaniac,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophile".Translate(),
                PSIMaterials[Icon.Prosthophile],
                ref ColBarSettings.ShowProsthophile,
                ref PsiSettings.ShowProsthophile,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Prosthophobe".Translate(),
                PSIMaterials[Icon.Prosthophobe],
                ref ColBarSettings.ShowProsthophobe,
                ref PsiSettings.ShowProsthophobe,
                ref num);
            this.DrawCheckboxArea(
                "PSI.Settings.Visibility.Pacific".Translate(),
                PSIMaterials[Icon.Pacific],
                ref ColBarSettings.ShowPacific,
                ref PsiSettings.ShowPacific,
                ref num);

            // DrawCheckboxArea("PSI.Settings.Visibility.Marriage".Translate(), PSIMaterials[Icons.Marriage], ref ColBarSettings.ShowMarriage, ref PsiSettings.ShowMarriage, ref num);
            GUILayout.EndHorizontal();

            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.EndScrollView();
        }

        private void FillPagePSIOpacityAndColor()
        {
            this.scrollPositionPSIOp = GUILayout.BeginScrollView(this.scrollPositionPSIOp);

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.Opacity".Translate() + (PsiSettings.IconOpacity * 100).ToString("N0")
                + " %");
            PsiSettings.IconOpacity = GUILayout.HorizontalSlider(PsiSettings.IconOpacity, 0.1f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.IconOpacityAndColor.OpacityCritical".Translate()
                + (PsiSettings.IconOpacityCritical * 100).ToString("N0") + " %");
            PsiSettings.IconOpacityCritical = GUILayout.HorizontalSlider(PsiSettings.IconOpacityCritical, 0.1f, 1f);
            GUILayout.EndVertical();

            // if (listing.DoTextButton("PSI.Settings.ResetColors".Translate()))
            // {
            // colorRedAlert = baseSettings.ColorRedAlert;
            // Scribe_Values.LookValue(ref colorRedAlert, "colorRedAlert");
            // colorInput.Value = colorRedAlert;
            // PSI.SaveSettings();
            // }
            // Rect row = new Rect(0f, listing.CurHeight, listing.ColumnWidth(), 24f);
            // DrawMCMRegion(row);
            // PSI.Settings.ColorRedAlert = colorInput.Value;
            // listing.DoGap();
            // listing.DoGap();
            GUILayout.EndScrollView();

            // if (listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            // Page = "main";
        }

        private void FillPSIPageSensitivity()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PSI.Settings.LoadPresetButton".Translate()))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>
                                                    {
                                                        new FloatMenuOption(
                                                            "Less Sensitive",
                                                            () =>
                                                                {
                                                                    try
                                                                    {
                                                                        PsiSettings.LimitBleedMult
                                                                            = 2f;
                                                                        PsiSettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        PsiSettings
                                                                                .LimitEfficiencyLess =
                                                                            0.6f;
                                                                        PsiSettings.LimitFoodLess
                                                                            = 0.2f;

                                                                        // PsiSettings.LimitMoodLess = 0.2f;
                                                                        PsiSettings.LimitRestLess
                                                                            = 0.2f;
                                                                        PsiSettings
                                                                                .LimitTempComfortOffset
                                                                            = 3f;
                                                                    }
                                                                    catch (IOException)
                                                                    {
                                                                        Log.Error(
                                                                            "PSI.Settings.LoadPreset.UnableToLoad"
                                                                                .Translate()
                                                                            + "Less Sensitive");
                                                                    }
                                                                }),
                                                        new FloatMenuOption(
                                                            "Standard",
                                                            () =>
                                                                {
                                                                    try
                                                                    {
                                                                        PsiSettings.LimitBleedMult
                                                                            = 3f;
                                                                        PsiSettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        PsiSettings
                                                                                .LimitEfficiencyLess =
                                                                            0.75f;
                                                                        PsiSettings.LimitFoodLess
                                                                            = 0.25f;

                                                                        // PsiSettings.LimitMoodLess = 0.25f;
                                                                        PsiSettings.LimitRestLess
                                                                            = 0.25f;
                                                                        PsiSettings
                                                                                .LimitTempComfortOffset
                                                                            = 0f;
                                                                    }
                                                                    catch (IOException)
                                                                    {
                                                                        Log.Error(
                                                                            "PSI.Settings.LoadPreset.UnableToLoad"
                                                                                .Translate()
                                                                            + "Standard");
                                                                    }
                                                                }),
                                                        new FloatMenuOption(
                                                            "More Sensitive",
                                                            () =>
                                                                {
                                                                    try
                                                                    {
                                                                        PsiSettings.LimitBleedMult
                                                                            = 4f;
                                                                        PsiSettings
                                                                                .LimitDiseaseLess =
                                                                            1f;
                                                                        PsiSettings
                                                                                .LimitEfficiencyLess =
                                                                            0.9f;
                                                                        PsiSettings.LimitFoodLess
                                                                            = 0.3f;

                                                                        // PsiSettings.LimitMoodLess = 0.3f;
                                                                        PsiSettings.LimitRestLess
                                                                            = 0.3f;
                                                                        PsiSettings
                                                                                .LimitTempComfortOffset
                                                                            = -3f;
                                                                    }
                                                                    catch (IOException)
                                                                    {
                                                                        Log.Error(
                                                                            "PSI.Settings.LoadPreset.UnableToLoad"
                                                                                .Translate()
                                                                            + "More Sensitive");
                                                                    }
                                                                })
                                                    };

                Find.WindowStack.Add(new FloatMenu(options));
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            this.scrollPositionPSISens = GUILayout.BeginScrollView(this.scrollPositionPSISens);

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Bleeding".Translate()
                + ("PSI.Settings.Sensitivity.Bleeding." + Math.Round(PsiSettings.LimitBleedMult - 0.25)).Translate());
            PsiSettings.LimitBleedMult = GUILayout.HorizontalSlider(PsiSettings.LimitBleedMult, 0.5f, 5f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Injured".Translate() + (int)(PsiSettings.LimitEfficiencyLess * 100.0) + " %");
            PsiSettings.LimitEfficiencyLess = GUILayout.HorizontalSlider(PsiSettings.LimitEfficiencyLess, 0.01f, 1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Food".Translate() + (int)(PsiSettings.LimitFoodLess * 100.0) + " %");
            PsiSettings.LimitFoodLess = GUILayout.HorizontalSlider(PsiSettings.LimitFoodLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Rest".Translate() + (int)(PsiSettings.LimitRestLess * 100.0) + " %");
            PsiSettings.LimitRestLess = GUILayout.HorizontalSlider(PsiSettings.LimitRestLess, 0.01f, 0.99f);
            GUILayout.EndVertical();

            // Replaced with thought check => human leather, dead man's apparel
            // GUILayout.BeginVertical(this._fondBoxes);
            // GUILayout.Label(
            // "PSI.Settings.Sensitivity.ApparelHealth".Translate() + (int)(PsiSettings.LimitApparelHealthLess * 100.0)
            // + " %");
            // PsiSettings.LimitApparelHealthLess =
            // GUILayout.HorizontalSlider(PsiSettings.LimitApparelHealthLess, 0.01f, 0.99f);
            // GUILayout.EndVertical();
            GUILayout.BeginVertical(this.fondBoxes);
            GUILayout.Label(
                "PSI.Settings.Sensitivity.Temperature".Translate() + (int)PsiSettings.LimitTempComfortOffset + " °C");
            PsiSettings.LimitTempComfortOffset =
                GUILayout.HorizontalSlider(PsiSettings.LimitTempComfortOffset, -10f, 10f);
            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            // if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
            // return;
            // Page = "main";
        }

        private void FillPSIPageSizeArrangement()
        {
            this.scrollPositionPSISize = GUILayout.BeginScrollView(this.scrollPositionPSISize);

            GUILayout.BeginVertical(this.fondBoxes);
            ColBarSettings.UsePsi = GUILayout.Toggle(ColBarSettings.UsePsi, "CBKF.Settings.UsePsiOnBar".Translate());
            if (ColBarSettings.UsePsi)
            {
                GUILayout.BeginHorizontal();
                this.PsiBarPositionInt = GUILayout.Toolbar(this.PsiBarPositionInt, this.positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + ColBarSettings.IconsInColumn);
                ColBarSettings.IconsInColumn = (int)GUILayout.HorizontalSlider(ColBarSettings.IconsInColumn, 2f, 5f);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(this.fondBoxes);
            PsiSettings.UsePsi = GUILayout.Toggle(PsiSettings.UsePsi, "PSI.Settings.UsePSI".Translate());
            PsiSettings.ShowRelationsOnStrangers = GUILayout.Toggle(
                PsiSettings.ShowRelationsOnStrangers,
                "PSI.Settings.ShowRelationsOnStrangers".Translate());
            PsiSettings.UsePsiOnPrisoner = GUILayout.Toggle(
                PsiSettings.UsePsiOnPrisoner,
                "PSI.Settings.UsePSIOnPrisoner".Translate());
            PsiSettings.UsePsiOnAnimals = GUILayout.Toggle(
                PsiSettings.UsePsiOnAnimals,
                "PSI.Settings.UsePsiOnAnimals".Translate());

            if (PsiSettings.UsePsi || PsiSettings.UsePsiOnPrisoner)
            {
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginHorizontal();
                this.PsiPositionInt = GUILayout.Toolbar(this.PsiPositionInt, this.positionStrings);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(Text.LineHeight / 2);

                PsiSettings.IconsHorizontal = GUILayout.Toggle(
                    PsiSettings.IconsHorizontal,
                    "PSI.Settings.Arrangement.Horizontal".Translate());

                PsiSettings.IconsScreenScale = GUILayout.Toggle(
                    PsiSettings.IconsScreenScale,
                    "PSI.Settings.Arrangement.ScreenScale".Translate());

                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.Label("PSI.Settings.Arrangement.IconsPerColumn".Translate() + PsiSettings.IconsInColumn);
                PsiSettings.IconsInColumn = (int)GUILayout.HorizontalSlider(PsiSettings.IconsInColumn, 1f, 7f);

                int num = (int)(PsiSettings.IconSize * 4.5);

                if (num > 8)
                {
                    num = 8;
                }
                else if (num < 0)
                {
                    num = 0;
                }

                GUILayout.Space(Text.LineHeight / 2);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconSize".Translate() + ("PSI.Settings.SizeLabel." + num).Translate());
                PsiSettings.IconSize = GUILayout.HorizontalSlider(PsiSettings.IconSize, 0.5f, 2f);
                GUILayout.Space(Text.LineHeight / 2);

                GUILayout.BeginVertical(this.fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconPosition".Translate() + (int)(PsiSettings.IconMarginX * 100.0)
                    + " x, " + (int)(PsiSettings.IconMarginY * 100.0) + " y");
                PsiSettings.IconMarginX = GUILayout.HorizontalSlider(PsiSettings.IconMarginX, -2f, 2f);
                PsiSettings.IconMarginY = GUILayout.HorizontalSlider(PsiSettings.IconMarginY, -2f, 2f);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(this.fondBoxes);
                GUILayout.Label(
                    "PSI.Settings.Arrangement.IconOffset".Translate() + (int)(PsiSettings.IconOffsetX * 100.0) + " x, "
                    + (int)(PsiSettings.IconOffsetY * 100.0) + " y");
                PsiSettings.IconOffsetX = GUILayout.HorizontalSlider(PsiSettings.IconOffsetX, -2f, 2f);
                PsiSettings.IconOffsetY = GUILayout.HorizontalSlider(PsiSettings.IconOffsetY, -2f, 2f);
                GUILayout.EndVertical();

                // if (!listing.DoTextButton("PSI.Settings.ReturnButton".Translate()))
                // return;
                // Page = "main";
            }

            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        private void LabelHeadline(string labelstring)
        {
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.Label(string.Empty, this.grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 4);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(labelstring, this.fontBold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(Text.LineHeight / 2);
            GUILayout.Label(string.Empty, this.grayLines, GUILayout.Height(1));
            GUILayout.Space(Text.LineHeight / 2);
        }

        private void ResetBarSettings()
        {
            ColBarSettings = new SettingsColonistBar();
        }

        private void ResetPSISettings()
        {
            PsiSettings = new SettingsPSI();
        }

        #endregion Private Methods

    }
}