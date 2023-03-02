using ModEngine2ConfigTool.ViewModels.Profiles;

namespace ModEngine2ConfigTool.Services.Interfaces
{
    public interface ISaveManagerService
    {
        void BackupSaves(string profileId);
        void CopySaves(string oldProfileName, string newProfileName);
        void CreateBackups(string profileId);
        void CreateUnmoddedBackups();
        void DeleteSaves(string profileId);
        void ImportSave(string profileId);
        void InstallProfileSaves(string profileName);
        void OpenProfileSavesFolder(string profileId);
        bool ProfileHasSaves(string profileId);
        void UninstallProfileSaves(string profileName);

        //new
        void BackupVanilla();

        void BackupProfile(IProfileVm profile);

        void Push(IProfileVm profile);

        void Pop(IProfileVm profile);
    }
}