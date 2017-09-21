﻿namespace ColonistBarKF.Bar
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    using RimWorld;
    using RimWorld.Planet;

    using UnityEngine;

    using Verse;

    // ReSharper disable once InconsistentNaming
    public class ColBarHelper_KF : IExposable
    {
        #region Fields

        [NotNull]
        public readonly List<Vector2> cachedDrawLocs = new List<Vector2>();

        [NotNull]
        private readonly List<EntryKF> cachedEntries = new List<EntryKF>();

        public float cachedScale;

        public bool EntriesDirty = true;

        [NotNull]
        public List<Pawn> tmpCaravanPawns = new List<Pawn>();

        [NotNull]
        public List<Caravan> tmpCaravans = new List<Caravan>();

        [NotNull]
        public List<Thing> tmpColonists = new List<Thing>();

        [NotNull]
        public List<Pawn> tmpColonistsInOrder = new List<Pawn>();

        [NotNull]
        public List<Pair<Thing, Map>> tmpColonistsWithMap = new List<Pair<Thing, Map>>();

        [NotNull]
        public List<Thing> tmpMapColonistsOrCorpsesInScreenRect = new List<Thing>();

        [NotNull]
        private List<Map> tmpMaps = new List<Map>();

        [NotNull]
        private List<Pawn> tmpPawns = new List<Pawn>();

        #endregion Fields

        #region Properties

        [NotNull]
        public List<Vector2> DrawLocs => this.cachedDrawLocs;

        [NotNull]
        public List<EntryKF> Entries
        {
            get
            {
                this.CheckRecacheEntries();
                return this.cachedEntries;
            }
        }

        #endregion Properties

        #region Methods

        public bool AnyBarEntryAt(Vector2 pos)
        {
            if (!this.TryGetEntryAt(pos, out EntryKF entry))
            {
                return false;
            }

            return entry.groupCount > 0;
        }

        private void CheckRecacheEntries()
        {
            if (!this.EntriesDirty)
            {
                return;
            }

            this.EntriesDirty = false;
            this.cachedEntries.Clear();
            if (Find.PlaySettings.showColonistBar)
            {
                this.tmpMaps.Clear();
                this.tmpMaps.AddRange(Find.Maps);
                this.tmpMaps.SortBy(x => !x.IsPlayerHome, x => x.uniqueID);
                int groupInt = 0;
                for (int i = 0; i < this.tmpMaps.Count; i++)
                {
                    this.tmpPawns.Clear();
                    this.tmpPawns.AddRange(this.tmpMaps[i].mapPawns.FreeColonists);
                    List<Thing> list = this.tmpMaps[i].listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (!list[j].IsDessicated())
                        {
                            Pawn innerPawn = ((Corpse)list[j]).InnerPawn;
                            if (innerPawn != null)
                            {
                                if (innerPawn.IsColonist)
                                {
                                    this.tmpPawns.Add(innerPawn);
                                }
                            }
                        }
                    }

                    List<Pawn> allPawnsSpawned = this.tmpMaps[i].mapPawns.AllPawnsSpawned;
                    for (int k = 0; k < allPawnsSpawned.Count; k++)
                    {
                        Corpse corpse = allPawnsSpawned[k].carryTracker.CarriedThing as Corpse;
                        if (corpse != null && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist)
                        {
                            this.tmpPawns.Add(corpse.InnerPawn);
                        }
                    }

                    // tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                    SortCachedColonists(ref this.tmpPawns);
                    for (int l = 0; l < this.tmpPawns.Count; l++)
                    {
                        this.cachedEntries.Add(new EntryKF(this.tmpPawns[l], this.tmpMaps[i], groupInt, this.tmpPawns.Count));

                        if (Settings.ColBarSettings.UseGrouping && groupInt != this.displayGroupForBar)
                        {
                            if (this.cachedEntries.FindAll(x => x.@group == groupInt).Count > 2)
                            {
                                this.cachedEntries.Add(new EntryKF(null, this.tmpMaps[i], groupInt, this.tmpPawns.Count));
                                break;
                            }
                        }
                    }

                    if (!this.tmpPawns.Any())
                    {
                        this.cachedEntries.Add(new EntryKF(null, this.tmpMaps[i], groupInt, 0));
                    }

                    groupInt++;
                }

                this.tmpCaravans.Clear();
                this.tmpCaravans.AddRange(Find.WorldObjects.Caravans);
                this.tmpCaravans.SortBy(x => x.ID);
                for (int m = 0; m < this.tmpCaravans.Count; m++)
                {
                    if (this.tmpCaravans[m].IsPlayerControlled)
                    {
                        this.tmpPawns.Clear();
                        this.tmpPawns.AddRange(this.tmpCaravans[m].PawnsListForReading);

                        // tmpPawns.SortBy((Pawn x) => x.thingIDNumber);
                        SortCachedColonists(ref this.tmpPawns);
                        for (int n = 0; n < this.tmpPawns.Count; n++)
                        {
                            if (this.tmpPawns[n].IsColonist)
                            {
                                this.cachedEntries.Add(
                                    new EntryKF(
                                        this.tmpPawns[n],
                                        null,
                                        groupInt,
                                        this.tmpPawns.FindAll(x => x.IsColonist).Count));

                                if (Settings.ColBarSettings.UseGrouping && groupInt != this.displayGroupForBar)
                                {
                                    if (this.cachedEntries.FindAll(x => x.group == groupInt).Count > 2)
                                    {
                                        this.cachedEntries.Add(
                                            new EntryKF(
                                                null,
                                                null,
                                                groupInt,
                                                this.tmpPawns.FindAll(x => x.IsColonist).Count));
                                        break;
                                    }
                                }
                            }
                        }

                        groupInt++;
                    }
                }
            }

            // RecacheDrawLocs();
            ColonistBar_KF.drawer.Notify_RecachedEntries();
            this.tmpPawns.Clear();
            this.tmpMaps.Clear();
            this.tmpCaravans.Clear();
            ColonistBar_KF.drawLocsFinder.CalculateDrawLocs(this.cachedDrawLocs, out this.cachedScale);
        }

        public bool TryGetEntryAt(Vector2 pos, out EntryKF entry)
        {
            List<Vector2> drawLocs = this.cachedDrawLocs;
            List<EntryKF> entries = this.Entries;
            Vector2 size = ColonistBar_KF.FullSize;
            for (int i = 0; i < drawLocs.Count; i++)
            {
                Rect rect = new Rect(drawLocs[i].x, drawLocs[i].y, size.x, size.y);
                if (rect.Contains(pos))
                {
                    entry = entries[i];
                    return true;
                }
            }

            entry = default(EntryKF);
            return false;
        }

        private static void SortCachedColonists([NotNull] ref List<Pawn> tmpColonists)
        {
            IOrderedEnumerable<Pawn> orderedEnumerable = null;
            {
                switch (Settings.ColBarSettings.SortBy)
                {
                    case SettingsColonistBar.SortByWhat.vanilla:
                        tmpColonists.SortBy(x => x.thingIDNumber);
                        Settings.SaveBarSettings();
                        break;

                    case SettingsColonistBar.SortByWhat.byName:
                        tmpColonists.SortBy(x => x.LabelCap);
                        Settings.SaveBarSettings();
                        break;

                    case SettingsColonistBar.SortByWhat.sexage:
                        orderedEnumerable = tmpColonists.OrderBy(x => x.gender.GetLabel() != null)
                            .ThenBy(x => x.gender.GetLabel()).ThenBy(x => x?.ageTracker?.AgeBiologicalYears);
                        tmpColonists = orderedEnumerable.ToList();
                        Settings.SaveBarSettings();
                        break;

                    case SettingsColonistBar.SortByWhat.health:
                        tmpColonists.SortBy(x => x.health.summaryHealth.SummaryHealthPercent);
                        Settings.SaveBarSettings();
                        break;

                    case SettingsColonistBar.SortByWhat.mood:
                        orderedEnumerable = tmpColonists.OrderBy(x => x?.needs?.mood?.CurInstantLevelPercentage);
                        tmpColonists = orderedEnumerable.ToList();

                        // tmpColonists.SortBy(x => x.needs.mood.CurLevelPercentage);
                        Settings.SaveBarSettings();
                        break;

                    case SettingsColonistBar.SortByWhat.weapons:
                        orderedEnumerable = tmpColonists
                            .OrderByDescending(
                                a => a?.equipment?.Primary?.def != null && a.equipment.Primary.def.IsMeleeWeapon)
                            .ThenByDescending(c => c?.equipment?.Primary?.def?.IsRangedWeapon).ThenByDescending(
                                b => b?.skills?.AverageOfRelevantSkillsFor(WorkTypeDefOf.Hunting));
                        tmpColonists = orderedEnumerable.ToList();
                        Settings.SaveBarSettings();
                        break;

                    // skill not really relevant
                    // case SettingsColonistBar.SortByWhat.medic:
                    // orderedEnumerable = tmpColonists.OrderBy(b => b?.skills != null).ThenByDescending(b => b?.skills.AverageOfRelevantSkillsFor(WorkTypeDefOf.Doctor));
                    // tmpColonists = orderedEnumerable.ToList();
                    // SaveBarSettings();
                    // break;
                    case SettingsColonistBar.SortByWhat.medicTendQuality:
                        tmpColonists.SortByDescending(b => b.GetStatValue(StatDefOf.MedicalTendQuality));
                        Settings.SaveBarSettings();
                        break;

                    case SettingsColonistBar.SortByWhat.medicSurgerySuccess:
                        tmpColonists.SortByDescending(b => b.GetStatValue(StatDefOf.MedicalSurgerySuccessChance));
                        Settings.SaveBarSettings();
                        break;

                    case SettingsColonistBar.SortByWhat.tradePrice:
                        tmpColonists.SortByDescending(b => b.GetStatValue(StatDefOf.TradePriceImprovement));
                        Settings.SaveBarSettings();
                        break;

                    default:
                        tmpColonists.SortBy(x => x.thingIDNumber);
                        Settings.SaveBarSettings();
                        break;
                }
            }
        }

        public int displayGroupForBar = 0;

        #endregion Methods

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.displayGroupForBar, "displayGroupForBar");
        }
    }
}