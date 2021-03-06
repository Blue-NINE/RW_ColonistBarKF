﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColonistBarKF.PSI;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using static ColonistBarKF.CBKF;
using static ColonistBarKF.ColonistBar_KF;

namespace ColonistBarKF.Detouring
{
    public static  class ColonistBarColonistDrawerKF
    {


        public static Vector2 PawnTextureSize = new Vector2(ColBarSettings.BaseSizeFloat - 2f, ColBarSettings.BaseSizeFloat * 1.5f);
        private static Vector2[] bracketLocs = new Vector2[4];
        public static void DrawColonist(Rect rect, Pawn colonist, Map pawnMap)
        {
            float entryRectAlpha = GetEntryRectAlpha(rect);
            ApplyEntryInAnotherMapAlphaFactor(pawnMap, ref entryRectAlpha);

            bool colonistAlive = (!colonist.Dead) ? Find.Selector.SelectedObjects.Contains(colonist) : Find.Selector.SelectedObjects.Contains(colonist.Corpse);

            Color color = new Color(1f, 1f, 1f, entryRectAlpha);
            GUI.color = color;

            Color BGColor = new Color();



            Rect moodBorderRect = new Rect(rect);
            if (ColBarSettings.UseExternalMoodBar)
            {
                // draw mood border
                switch (ColBarSettings.MoodBarPos)
                {
                    case Position.Alignment.Right:
                        moodBorderRect.x = rect.xMax;
                        moodBorderRect.width /= 4;
                        break;
                    case Position.Alignment.Left:
                        moodBorderRect.x = rect.xMin - rect.width / 4;
                        moodBorderRect.width /= 4;
                        break;
                    case Position.Alignment.Top:
                        moodBorderRect.x = rect.xMin;
                        moodBorderRect.y = rect.yMin - rect.height / 4;
                        moodBorderRect.height /= 4;
                        break;
                    case Position.Alignment.Bottom:
                        moodBorderRect.x = rect.xMin;
                        moodBorderRect.y = moodBorderRect.yMax + SpacingLabel;
                        moodBorderRect.height /= 4;
                        break;
                }
            }

            PawnStats pawnStats = null;
            if (ColBarSettings.UseNewMood || ColBarSettings.UseExternalMoodBar)
            {
                if (PSI.PSI._statsDict.TryGetValue(colonist, out pawnStats))
                    if (pawnStats.mood != null && pawnStats.mb != null)
                    {
                        if (pawnStats.mood.CurLevelPercentage <= pawnStats.mb.BreakThresholdExtreme)
                        {
                            GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                            GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodExtremeCrossedTex);
                        }
                        else if (pawnStats.mood.CurLevelPercentage <= pawnStats.mb.BreakThresholdMajor)
                        {
                            GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                            GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodMajorCrossedTex);
                        }
                        else if (pawnStats.mood.CurLevelPercentage <= pawnStats.mb.BreakThresholdMinor)
                        {
                            GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                            GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodMinorCrossedTex);
                        }
                        else
                        {
                            GUI.DrawTexture(moodBorderRect, ColonistBarTextures.MoodNeutral);
                        }
                    }
            }



            if (ColBarSettings.UseGender)
            {
                if (colonist.gender == Gender.Male)
                {
                    BGColor = ColBarSettings.MaleColor;
                }
                if (colonist.gender == Gender.Female)
                {
                    BGColor = ColBarSettings.FemaleColor;
                }

                if (colonist.Dead)
                    BGColor = BGColor * Color.gray;

                BGColor.a = entryRectAlpha;

                GUI.color = BGColor;
            }


            GUI.DrawTexture(rect, ColBarSettings.UseGender ? ColonistBarTextures.BGTexGrey : ColonistBarTextures.BGTexVanilla);

            GUI.color = color;

            if (colonist.needs != null && colonist.needs.mood != null)
            {
                if (ColBarSettings.UseExternalMoodBar || ColBarSettings.UseNewMood)
                {
                    if (pawnStats != null)
                    {
                        Rect moodRect = moodBorderRect.ContractedBy(2f);
                        DrawNewMoodRect(moodRect, pawnStats.mood, pawnStats.mb);
                    }
                }
                else
                {
                    Rect position = rect.ContractedBy(2f);
                    float num = position.height * colonist.needs.mood.CurLevelPercentage;
                    position.yMin = position.yMax - num;
                    position.height = num;
                    GUI.DrawTexture(position, ColonistBarTextures.MoodBGTex);
                }
            }



            Rect rect2 = rect.ContractedBy(-2f * Scale);

            if (colonistAlive && !WorldRendererUtility.WorldRenderedNow)
            {
                DrawSelectionOverlayOnGUI(colonist, rect2);
            }
            else if (WorldRendererUtility.WorldRenderedNow && colonist.IsCaravanMember() && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
            {
                DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
            }

            GUI.DrawTexture(GetPawnTextureRect(rect.x, rect.y), PortraitsCache.Get(colonist, PawnTextureSize, PawnTextureCameraOffset, ColBarSettings.PawnTextureCameraZoom));

            if (ColBarSettings.UseWeaponIcons)
            {
                DrawWeaponIcon(rect, colonist);
            }

            GUI.color = new Color(1f, 1f, 1f, entryRectAlpha * 0.8f);
            DrawIcons(rect, colonist);
            GUI.color = color;
            if (colonist.Dead)
            {
                GUI.DrawTexture(rect, ColonistBarTextures.DeadColonistTex);
            }
            //       float num = 4f * Scale;
            Vector2 pos = new Vector2(rect.center.x, rect.yMax + 1f * Scale);
            GenMapUI.DrawPawnLabel(colonist, pos, entryRectAlpha, rect.width + ColonistBarDrawLocsFinderKF.SpacingHorizontal - 2f, ColonistBarColonistDrawerKF.pawnLabelsCache);
            GUI.color = Color.white;
        }

        private static Vector3 _pawnTextureCameraOffset;

        public static Vector3 PawnTextureCameraOffset
        {
            get
            {
                float pawnTextureCameraOffsetNew = ColBarSettings.PawnTextureCameraZoom / 1.28205f;
                _pawnTextureCameraOffset = new Vector3(ColBarSettings.PawnTextureCameraHorizontalOffset / pawnTextureCameraOffsetNew, 0f, ColBarSettings.PawnTextureCameraVerticalOffset / pawnTextureCameraOffsetNew);
                return _pawnTextureCameraOffset;
            }

            set { _pawnTextureCameraOffset = value; }
        }

        private static Rect GetPawnTextureRect(float x, float y)
        {
            Vector2 vector = ColonistBarColonistDrawerKF.PawnTextureSize * Scale;
            return new Rect(x + 1f, y - (vector.y - Size.y) - 1f, vector.x, vector.y);
        }

        private static void DrawIcons(Rect rect, Pawn colonist)
        {
            if (colonist.Dead)
            {
                return;
            }


            Vector2 vector = new Vector2(rect.x + 1f, rect.yMax - rect.width / 5 * 2 - 1f);
            bool attacking = false;
            if (colonist.CurJob != null)
            {
                JobDef def = colonist.CurJob.def;
                if (def == JobDefOf.AttackMelee || def == JobDefOf.AttackStatic)
                {
                    attacking = true;
                }
                else if (def == JobDefOf.WaitCombat)
                {
                    Stance_Busy stance_Busy = colonist.stances.curStance as Stance_Busy;
                    if (stance_Busy != null && stance_Busy.focusTarg.IsValid)
                    {
                        attacking = true;
                    }
                }
            }
            if (colonist.InAggroMentalState)
            {
                //        DrawIcon(PSI.PSI.PSIMaterials[Icons.Aggressive].mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                DrawIcon(ColonistBarTextures.Icon_MentalStateAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InMentalState)
            {
                //        DrawIcon(PSI.PSI.PSIMaterials[Icons.Dazed].mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                DrawIcon(ColonistBarTextures.Icon_MentalStateNonAggro, ref vector, colonist.MentalStateDef.LabelCap);
            }
            else if (colonist.InBed() && colonist.CurrentBed().Medical)
            {
                //        DrawIcon(PSI.PSI.PSIMaterials[Icons.Health].mainTexture as Texture2D, ref vector, "ActivityIconMedicalRest".Translate());
                DrawIcon(ColonistBarTextures.Icon_MedicalRest, ref vector, "ActivityIconMedicalRest".Translate());
            }
            else if (colonist.CurJob != null && colonist.jobs.curDriver.asleep)
            {
                //      DrawIcon(PSI.PSI.PSIMaterials[Icons.Tired].mainTexture as Texture2D, ref vector, "ActivityIconSleeping".Translate());
                DrawIcon(ColonistBarTextures.Icon_Sleeping, ref vector, "ActivityIconSleeping".Translate());
            }
            else if (colonist.CurJob != null && colonist.CurJob.def == JobDefOf.FleeAndCower)
            {
                //      DrawIcon(PSI.PSI.PSIMaterials[Icons.Leave].mainTexture as Texture2D, ref vector, "ActivityIconFleeing".Translate());
                DrawIcon(ColonistBarTextures.Icon_Fleeing, ref vector, "ActivityIconFleeing".Translate());
            }
            else if (attacking)
            {
                DrawIcon(ColonistBarTextures.Icon_Attacking, ref vector, "ActivityIconAttacking".Translate());
            }
            else if (colonist.mindState.IsIdle && GenDate.DaysPassed >= 1)
            {
                //  DrawIcon(PSI.PSI.PSIMaterials[Icons.Idle].mainTexture as Texture2D, ref vector, "ActivityIconIdle".Translate());
                DrawIcon(ColonistBarTextures.Icon_Idle, ref vector, "ActivityIconIdle".Translate());
            }
            if (false)
            {
                PawnStats pawnStats;
                if (colonist.Dead || colonist.holdingContainer != null || !PSI.PSI._statsDict.TryGetValue(colonist, out pawnStats) ||
                    colonist.drafter == null || colonist.skills == null)
                    return;

                if (pawnStats.MentalSanity == MentalStateDefOf.BingingDrugMajor || pawnStats.MentalSanity == MentalStateDefOf.BingingDrugExtreme)
                {
                    Material material = PSI.PSI.PSIMaterials[Icons.Drunk];
                    DrawIcon(material.mainTexture as Texture2D, ref vector, colonist.MentalStateDef.LabelCap);
                }
            }

            if (colonist.IsBurning())
            {
                DrawIcon(ColonistBarTextures.Icon_Burning, ref vector, "ActivityIconBurning".Translate());
            }
        }

        private static void DrawIcon(Texture2D icon, ref Vector2 pos, string tooltip)
        {
            float num = ColBarSettings.BaseSizeFloat * 0.4f * Scale;
            Rect rect = new Rect(pos.x, pos.y, num, num);
            GUI.DrawTexture(rect, icon);
            TooltipHandler.TipRegion(rect, tooltip);
            pos.x += num;
        }

        private static void DrawWeaponIcon(Rect rect, Pawn colonist)
        {
            float entryRectAlpha = GetEntryRectAlpha(rect);
            ColonistBarColonistDrawerKF.ApplyEntryInAnotherMapAlphaFactor(colonist.Map, ref entryRectAlpha);
            Color color = new Color(1f, 1f, 1f, entryRectAlpha);
            GUI.color = color;
            if (colonist?.equipment.Primary != null)
            {
                var thing = colonist.equipment.Primary;
                Rect rect2 = rect.ContractedBy(rect.width / 3);

                rect2.x = rect.xMax - rect2.width - rect.width / 12;
                rect2.y = rect.yMax - rect2.height - rect.height / 12;

                GUI.color = color;
                Texture2D resolvedIcon;
                if (!thing.def.uiIconPath.NullOrEmpty())
                {
                    resolvedIcon = thing.def.uiIcon;
                }
                else
                {
                    resolvedIcon = thing.Graphic.ExtractInnerGraphicFor(thing).MatSingle.mainTexture as Texture2D;
                }
                // color labe by thing
                Color iconcolor = new Color();

                if (thing.def.IsMeleeWeapon)
                {
                    GUI.color = new Color(0.85f, 0.2f, 0.2f, entryRectAlpha);
                    iconcolor = new Color(0.2f, 0.05f, 0.05f, 0.95f * entryRectAlpha);
                }
                if (thing.def.IsRangedWeapon)
                {
                    GUI.color = new Color(0.15f, 0.3f, 0.85f, entryRectAlpha);
                    iconcolor = new Color(0.03f, 0.075f, 0.2f, 0.95f * entryRectAlpha);
                }
                Widgets.DrawBoxSolid(rect2, iconcolor);
                Widgets.DrawBox(rect2);
                GUI.color = color;
                Rect rect3 = rect2.ContractedBy(rect2.width / 8);

                Widgets.DrawTextureRotated(rect3, resolvedIcon, 0);
                if (Mouse.IsOver(rect2))
                {
                    GUI.color = HighlightColor;
                    GUI.DrawTexture(rect2, TexUI.HighlightTex);
                }
                TooltipHandler.TipRegion(rect2, thing.def.LabelCap);

            }
        }

        private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);


        private static void DrawSelectionOverlayOnGUI(Pawn colonist, Rect rect)
        {
            Thing obj = colonist;
            if (colonist.Dead)
            {
                obj = colonist.Corpse;
            }
            float num = 0.4f * Scale;
            Vector2 textureSize = new Vector2(SelectionDrawerUtility.SelectedTexGUI.width * num, SelectionDrawerUtility.SelectedTexGUI.height * num);
            SelectionDrawerUtility.CalculateSelectionBracketPositionsUI(bracketLocs, obj, rect, SelectionDrawer.SelectTimes, textureSize, ColBarSettings.BaseSizeFloat * 0.4f * Scale);
            DrawSelectionOverlayOnGUI(bracketLocs, num);
        }

        private static void DrawSelectionOverlayOnGUI(Vector2[] bracketLocs, float selectedTexScale)
        {
            int num = 90;
            for (int i = 0; i < 4; i++)
            {
                Widgets.DrawTextureRotated(bracketLocs[i], SelectionDrawerUtility.SelectedTexGUI, num, selectedTexScale);
                num += 90;
            }
        }

        private static void DrawNewMoodRect(Rect moodRect, Need_Mood mood, MentalBreaker mb)
        {

            if (mood != null && mb != null)
            {
                if (mood.CurLevelPercentage > mb.BreakThresholdMinor)
                {
                    if (ColBarSettings.MoodBarPos == Position.Alignment.Left || ColBarSettings.MoodBarPos == Position.Alignment.Right)
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodTex);
                    else
                        GUI.DrawTexture(moodRect.LeftPart(mood.CurLevelPercentage), ColonistBarTextures.MoodTex);
                }
                else if (mood.CurLevelPercentage > mb.BreakThresholdMajor)
                {
                    if (ColBarSettings.MoodBarPos == Position.Alignment.Left || ColBarSettings.MoodBarPos == Position.Alignment.Right)
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodMinorCrossedTex);
                    else
                        GUI.DrawTexture(moodRect.LeftPart(mood.CurLevelPercentage), ColonistBarTextures.MoodMinorCrossedTex);
                }
                else if (mood.CurLevelPercentage > mb.BreakThresholdExtreme)
                {
                    if (ColBarSettings.MoodBarPos == Position.Alignment.Left || ColBarSettings.MoodBarPos == Position.Alignment.Right)
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodMajorCrossedTex);
                    else
                        GUI.DrawTexture(moodRect.LeftPart(mood.CurLevelPercentage), ColonistBarTextures.MoodMajorCrossedTex);
                }
                else
                {
                    GUI.DrawTexture(moodRect, ColonistBarTextures.MoodExtremeCrossedBGTex);
                    if (ColBarSettings.MoodBarPos == Position.Alignment.Left || ColBarSettings.MoodBarPos == Position.Alignment.Right)
                        GUI.DrawTexture(moodRect.BottomPart(mood.CurLevelPercentage), ColonistBarTextures.MoodExtremeCrossedTex);
                    else
                        GUI.DrawTexture(moodRect.LeftPart(mood.CurLevelPercentage), ColonistBarTextures.MoodExtremeCrossedTex);
                }

                DrawMentalThreshold(moodRect, mb.BreakThresholdExtreme, mood.CurLevelPercentage);
                DrawMentalThreshold(moodRect, mb.BreakThresholdMajor, mood.CurLevelPercentage);
                DrawMentalThreshold(moodRect, mb.BreakThresholdMinor, mood.CurLevelPercentage);

                switch (ColBarSettings.MoodBarPos)
                {
                    case Position.Alignment.Left:
                    case Position.Alignment.Right:
                        GUI.DrawTexture(
                            new Rect(moodRect.x, moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage, moodRect.width,
                                1), ColonistBarTextures.MoodTargetTex);
                        GUI.DrawTexture(
                            new Rect(moodRect.xMax + 1, moodRect.yMax - moodRect.height * mood.CurInstantLevelPercentage - 1, 2, 3),
                            ColonistBarTextures.MoodTargetTex);
                        break;
                    case Position.Alignment.Top:
                        GUI.DrawTexture(
                            new Rect(moodRect.x + moodRect.width * mood.CurInstantLevelPercentage, moodRect.y, 1, moodRect.height),
                            ColonistBarTextures.MoodTargetTex);
                        GUI.DrawTexture(
                            new Rect(moodRect.x + moodRect.width * mood.CurInstantLevelPercentage - 1, moodRect.yMin - 1, 3, 2),
                            ColonistBarTextures.MoodTargetTex);
                        break;
                    case Position.Alignment.Bottom:
                        GUI.DrawTexture(
                            new Rect(moodRect.x + moodRect.width * mood.CurInstantLevelPercentage, moodRect.y, 1, moodRect.height),
                            ColonistBarTextures.MoodTargetTex);
                        GUI.DrawTexture(
                            new Rect(moodRect.x + moodRect.width * mood.CurInstantLevelPercentage - 1, moodRect.yMax + 1, 3, 2),
                            ColonistBarTextures.MoodTargetTex);
                        break;
                }
            }
        }

        internal static void DrawMentalThreshold(Rect moodRect, float threshold, float currentMood)
        {
            if (ColBarSettings.MoodBarPos == Position.Alignment.Left || ColBarSettings.MoodBarPos == Position.Alignment.Right)
                GUI.DrawTexture(new Rect(moodRect.x, moodRect.yMax - moodRect.height * threshold, moodRect.width, 1), ColonistBarTextures.MoodBreakTex);
            else
                GUI.DrawTexture(new Rect(moodRect.x + moodRect.width * threshold, moodRect.y, 1, moodRect.height), ColonistBarTextures.MoodBreakTex);

            /*if (currentMood <= threshold)
			{
				GUI.DrawTexture(new Rect(moodRect.xMax-4, moodRect.yMax - moodRect.height * threshold, 8, 2), MoodBreakCrossedTex);
			}*/
        }



        public static float SpacingLabel = 20f;

        // RimWorld.ColonistBarColonistDrawer
        private static void DrawCaravanSelectionOverlayOnGUI(Caravan caravan, Rect rect)
        {
            float num = 0.4f * Scale;
            Vector2 textureSize = new Vector2(SelectionDrawerUtility.SelectedTexGUI.width * num, SelectionDrawerUtility.SelectedTexGUI.height * num);
            SelectionDrawerUtility.CalculateSelectionBracketPositionsUI(bracketLocs, caravan, rect, WorldSelectionDrawer.SelectTimes, textureSize, 20f * Scale);
            DrawSelectionOverlayOnGUI(bracketLocs, num);
        }


        // RimWorld.ColonistBarColonistDrawer
        public static void ApplyEntryInAnotherMapAlphaFactor(Map map, ref float alpha)
        {
            if (map == null)
            {
                if (!WorldRendererUtility.WorldRenderedNow)
                {
                    alpha = Mathf.Min(alpha, 0.4f);
                }
            }
            else if (map != Find.VisibleMap || WorldRendererUtility.WorldRenderedNow)
            {
                alpha = Mathf.Min(alpha, 0.4f);
            }
        }
        public static Dictionary<string, string> pawnLabelsCache = new Dictionary<string, string>();

        public static void Notify_RecachedEntries()
        {
            pawnLabelsCache.Clear();
            RecacheDrawLocs();
        }

        // RimWorld.ColonistBarColonistDrawer
        public static Rect GroupFrameRect(int group)
        {
            float pos_x = 99999f;
            float width = 0f;
            float height = 0f;
            List<EntryKF> entries = Entries;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].group == group)
                {
                    pos_x = Mathf.Min(pos_x, cachedDrawLocs[i].x);
                    width = Mathf.Max(width, cachedDrawLocs[i].x + Size.x);
                    height = Mathf.Max(height, cachedDrawLocs[i].y + Size.y);
                }
            }
            return new Rect(pos_x, 0f, width - pos_x, height).ContractedBy(-12f * Scale);
        }



        // RimWorld.ColonistBarColonistDrawer
        public static void DrawGroupFrame(int group)
        {
            Rect position = GroupFrameRect(group);
            List<EntryKF> entries = Entries;
            Map map = entries.Find(x => x.group == group).map;
            float alpha;
            if (map == null)
            {
                if (WorldRendererUtility.WorldRenderedNow)
                {
                    alpha = 1f;
                }
                else
                {
                    alpha = 0.75f;
                }
            }
            else if (map != Find.VisibleMap || WorldRendererUtility.WorldRenderedNow)
            {
                alpha = 0.75f;
            }
            else
            {
                alpha = 1f;
            }
            Widgets.DrawRectFast(position, new Color(0.5f, 0.5f, 0.5f, 0.4f * alpha), null);
        }


        public static void RecacheDrawLocs()
        {
            Vector2 size = Size;
            int colonistsPerRow = ColonistBarDrawLocsFinderKF.ColonistsPerRow;
            float spacingHorizontal = ColonistBarDrawLocsFinderKF.SpacingHorizontal + ColonistBarDrawLocsFinderKF.SpacingPSIHorizontal + ColonistBarDrawLocsFinderKF.SpacingMoodBarHorizontal;
            float spacingVertical = ColonistBarDrawLocsFinderKF.SpacingVertical + ColonistBarDrawLocsFinderKF.SpacingPSIVertical + ColonistBarDrawLocsFinderKF.SpacingMoodBarVertical;


            float cachedDrawLocs_x = 0f + ColBarSettings.MarginLeftHorTop * Scale;
            float cachedDrawLocs_y = ColBarSettings.MarginTopHor * Scale;

            cachedDrawLocs.Clear();

            #region Vertical

            for (int i = 0; i < cachedEntries.Count; i++)
            {
                if (i % colonistsPerRow == 0)
                {
                    int maxColInRow = Mathf.Min(colonistsPerRow, cachedEntries.Count - i);
                    float num4 = maxColInRow * size.x + (maxColInRow - 1) * spacingHorizontal;
                    cachedDrawLocs_x = (Screen.width - num4) / 2f + ColBarSettings.HorizontalOffset;

                    if (ColBarSettings.UsePsi)
                        ModifyBasicDrawLocsForPsi(size, ref cachedDrawLocs_x, ref cachedDrawLocs_y);

                    if (ColBarSettings.UseExternalMoodBar)
                        ModifyBasicDrawLocsForMoodBar(size, ref cachedDrawLocs_x, ref cachedDrawLocs_y);


                    if (i != 0)
                    {
                        {
                            cachedDrawLocs_y += size.y + spacingVertical + ColonistBarColonistDrawerKF.SpacingLabel;
                        }
                    }
                }
                else
                {
                    cachedDrawLocs_x += size.x + spacingHorizontal;
                }
                cachedDrawLocs.Add(new Vector2(cachedDrawLocs_x, cachedDrawLocs_y));
            }


            #endregion


        }


        public static void ModifyBasicDrawLocsForPsi(Vector2 size, ref float cachedDrawLocs_x, ref float cachedDrawLocs_y)
        {
            switch (ColBarSettings.ColBarPsiIconPos)
            {
                case Position.Alignment.Left:
                    cachedDrawLocs_x += size.x / ColBarSettings.IconsInColumn * ColonistBarDrawLocsFinderKF.PsiRowsOnBar / 2;
                    break;

                case Position.Alignment.Right:
                    cachedDrawLocs_x -= size.x / ColBarSettings.IconsInColumn * ColonistBarDrawLocsFinderKF.PsiRowsOnBar / 2;
                    break;

                case Position.Alignment.Bottom:
                    //      cachedDrawLocs_y -= size.y/ColBarSettings.IconsInColumn*PsiRowsOnBar;
                    break;
                case Position.Alignment.Top:
                    cachedDrawLocs_y += size.y / ColBarSettings.IconsInColumn * ColonistBarDrawLocsFinderKF.PsiRowsOnBar;
                    break;
            }
        }

        public static void ModifyBasicDrawLocsForMoodBar(Vector2 size, ref float cachedDrawLocs_x, ref float cachedDrawLocs_y)
        {
            switch (ColBarSettings.MoodBarPos)
            {
                case Position.Alignment.Left:
                    cachedDrawLocs_x += size.x / 8;
                    break;

                case Position.Alignment.Right:
                    cachedDrawLocs_x -= size.x / 8;
                    break;

                case Position.Alignment.Top:
                    cachedDrawLocs_y += size.y / 8;
                    break;

                case Position.Alignment.Bottom:
                    //      cachedDrawLocs_y -= size.y/ColBarSettings.IconsInColumn*PsiRowsOnBar;
                    break;
            }
        }
    }
}
