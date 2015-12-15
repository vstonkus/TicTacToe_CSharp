using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class Parser
    {
        private static readonly Regex m_regex = new Regex("d(?<=d)1:v1:([xo0])1:xi([012])e1:yi([012])ee");

        public static string Encode(IEnumerable<Action> board)
        {
            return string.Format("l{0}e",
                string.Concat(
                    board.Select(
                        action => string.Format("d1:v1:{0}1:xi{1}e1:yi{2}ee", action.letter, action.x, action.y))));
        }

        public static IEnumerable<Action> Decode(string input)
        {
            return
                m_regex.Matches(input)
                    .Cast<Match>()
                    .Select(
                        action =>
                            new Action
                            {
                                letter = action.Groups[1].Value[0],
                                x = int.Parse(action.Groups[2].Value),
                                y = int.Parse(action.Groups[3].Value)
                            });
        }
    }
}
