namespace Solas.GameplayMod.Enums;

[Flags]
internal enum Weaknesses
{
    None = 0,
    NoWeaknesses = 1 << 0,

    OralTaker = 1 << 1,
    OralSuck = 1 << 2,
    OralLick = 1 << 3,

    HandJobs = 1 << 4,
    BreastPlay = 1 << 5,
    FootPlay = 1 << 6,

    SexPenetrate = 1 << 7,
    SexPenetrated = 1 << 8,
    SexLesbian = 1 << 9,

    SexMissionaryPenetrate = 1 << 10,
    SexMissionaryPenetrated = 1 << 11,
    SexMissionaryLesbian = 1 << 12,

    MissionaryPenetrate = SexPenetrate | SexMissionaryPenetrate,
    MissionaryPenetrated = SexPenetrated | SexMissionaryPenetrated,
    MissionaryLesbian = SexLesbian | SexMissionaryLesbian,

    SexDoggyPenetrate = 1 << 13,
    SexDoggyPenetrated = 1 << 14,
    SexDoggyLesbian = 1 << 15,

    DoggyPenetrate = SexPenetrate | SexDoggyPenetrate,
    DoggyPenetrated = SexPenetrated | SexDoggyPenetrated,
    DoggyLesbian = SexLesbian | SexDoggyLesbian,

    SexMountingPenetrate = 1 << 16,
    SexMountingPenetrated = 1 << 17,
    SexMountingLesbian = 1 << 18,

    MountingPenetrate = SexPenetrate | SexMountingPenetrate,
    MountingPenetrated = SexPenetrated | SexMountingPenetrated,
    MountingLesbian = SexLesbian | SexMountingLesbian,

    Threesome = 1 << 19,
    Bondage = 1 << 20,

    Orals = OralTaker | OralSuck | OralLick,
    MissionaryStyle = MissionaryPenetrate | MissionaryPenetrated | DoggyLesbian,
    DoggyStyle = DoggyPenetrate | DoggyPenetrated | DoggyLesbian,
    MountingStyle = MountingPenetrate | MountingPenetrated | MountingLesbian,
    SexStyle = MissionaryStyle | DoggyStyle | MountingStyle,
}
