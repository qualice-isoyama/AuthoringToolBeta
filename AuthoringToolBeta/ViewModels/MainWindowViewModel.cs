using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthoringToolBeta.ViewModels
{
    public class MainWindowViewModel
    {
        // UIにリストの変更を自動通知できる特殊なコレクション
        public ObservableCollection<string> Assets { get; }

        public MainWindowViewModel()
        {
            // 仮のアセットデータを作成
            Assets = new ObservableCollection<string>
            {
                "effect01.png",
                "character_a_01.png",
                "bgm_battle.wav",
                "sound_effect_win.mp3",
                "title_logo.png"
            };
        }
    }
}
