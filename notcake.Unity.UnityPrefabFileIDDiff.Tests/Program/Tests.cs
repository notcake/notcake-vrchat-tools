﻿using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace notcake.Unity.UnityPrefabFileIDDiff.Tests.Program
{
    using Program = notcake.Unity.UnityPrefabFileIDDiff.Program;

    /// <summary>
    ///     Tests for <see cref="notcake.Unity.UnityPrefabFileIDDiff"/>.
    /// </summary>
    [TestClass]
    [DeploymentItem("Resources/NestedPrefab1.prefab")]
    [DeploymentItem("Resources/NestedPrefab2.prefab")]
    [DeploymentItem("Resources/NestedPrefab3.prefab")]
    [DeploymentItem("Resources/NestedPrefab3a.prefab")]
    [DeploymentItem("Resources/GameObject.prefab")]
    [DeploymentItem("Resources/PrefabVariant.prefab")]
    [DeploymentItem("Resources/GameObjectAndPrefabInstanceComponentsA.prefab")]
    [DeploymentItem("Resources/GameObjectAndPrefabInstanceComponentsB.prefab")]
    [DeploymentItem("Resources/ReorderedComponentsA.prefab")]
    [DeploymentItem("Resources/ReorderedComponentsAReordered.prefab")]
    [DeploymentItem("Resources/ReorderedComponentsB.prefab")]
    [DeploymentItem("Resources/ReorderedComponentsBReordered.prefab")]
    [DeploymentItem("Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab")]
    [DeploymentItem("Resources/Avatars/Toastacuga.prefab")]
    public class Tests
    {
        /// <summary>
        ///     Tests the diff of a prefab file with itself.
        /// </summary>
        /// <param name="path">The path to the prefab file.</param>
        /// <param name="expectedLines">
        ///     The expected diff output, excluding trailing line breaks.
        /// </param>
        [DataTestMethod]
        [DataRow(
            "Resources/NestedPrefab1.prefab",
            new object[]
            {
                new[]
                {
                    "* Nested Prefab 1 &4989725120814738964",
                    "  * Transform &8820928727568499859",
                    "  * Child 1 &8294443650823064410",
                    "    * Transform &-4272491518163361196",
                    "  * Child 2 &5103454601818718131",
                    "    * Transform &2111712220508511864",
                }
            }
        )]
        [DataRow(
            "Resources/NestedPrefab2.prefab",
            new object[]
            {
                new[]
                {
                    "* Nested Prefab 2 &4989725120814738964",
                    "  * Transform &8820928727568499859",
                    "  * PrefabInstance &4156974465077255250",
                    "    * Transform &4889415719643437249 stripped",
                    "  * PrefabInstance &4431041678912474380",
                    "    * Transform &5121722109705652639 stripped",
                }
            }
        )]
        [DataRow(
            "Resources/NestedPrefab3.prefab",
            new object[]
            {
                new[]
                {
                    "* Nested Prefab 3 &4989725120814738964",
                    "  * Transform &8820928727568499859",
                    "  * PrefabInstance &384786867856306869",
                    "    * Transform &9168539870313657894 stripped",
                    "  * PrefabInstance &476074792185059992",
                    "    * Transform &9003083168696704523 stripped",
                    "    * Transform &2786369713623583212 stripped",
                    "      * New Inner Child &5073144092867333913",
                    "        * Transform &1912756242714561364",
                }
            }
        )]
        [DataRow(
            "Resources/NestedPrefab3a.prefab",
            new object[]
            {
                new[]
                {
                    "* Nested Prefab 3 &6293954053335516682",
                    "  * Transform &7497849768378543245",
                    "  * PrefabInstance &1670981476826454699",
                    "    * Transform &7879820769096490552 stripped",
                    "  * PrefabInstance &1512319774972293766",
                    "    * Transform &7968817328979179029 stripped",
                    "    * Transform &3804593009358545394 stripped",
                    "      * New Inner Child &6053085283548292871",
                    "        * Transform &643158201123562314",
                }
            }
        )]
        [DataRow(
            "Resources/GameObject.prefab",
            new object[]
            {
                new[]
                {
                    "* GameObject &5413307586876789807",
                    "  * Transform &5467125436267295398",
                }
            }
        )]
        [DataRow(
            "Resources/PrefabVariant.prefab",
            new object[]
            {
                new[]
                {
                    "* PrefabInstance &6612481006152796884",
                }
            }
        )]
        [DataRow(
            "Resources/GameObjectAndPrefabInstanceComponentsA.prefab",
            new object[]
            {
                new[]
                {
                    "* GameObjectAndPrefabInstanceComponents &1295435386291451790",
                    "  * Transform &6900024346194483700",
                    "  * Child 1 &3246267058468154075",
                    "    * Transform &446762813002330973",
                    "    * MonoBehaviour &2445294676174449731",
                    "    * MonoBehaviour &8101453579712206767",
                    "  * Child 2 &1130121813463608664",
                    "    * Transform &9040806159793680495",
                    "    * MonoBehaviour &1527149206699838427",
                    "    * MonoBehaviour &3122549251109195932",
                    "  * PrefabInstance &3053240162318261203",
                    "    * GameObject &7007784988793393148 stripped",
                    "      * MonoBehaviour &7167184517858377455",
                    "      * MonoBehaviour &9148366837940496496",
                    "    * Transform &7025730173712034165 stripped",
                    "  * PrefabInstance &6440009496554267807",
                    "    * GameObject &1315169930923323568 stripped",
                    "      * MonoBehaviour &6864714582965902295",
                    "      * MonoBehaviour &4575199936328865583",
                    "    * Transform &1333244823837382201 stripped",
                }
            }
        )]
        [DataRow(
            "Resources/GameObjectAndPrefabInstanceComponentsB.prefab",
            new object[]
            {
                new[]
                {
                    "* GameObjectAndPrefabInstanceComponents &3703353320363238006",
                    "  * Transform &9034067136953502732",
                    "  * Child 1 &1122439843563388707",
                    "    * Transform &2642913480352949925",
                    "    * MonoBehaviour &248213834871142843",
                    "    * MonoBehaviour &5976451239843250775",
                    "  * Child 2 &3256177755652880544",
                    "    * Transform &6911300203454047639",
                    "    * MonoBehaviour &4012493667704102435",
                    "    * MonoBehaviour &705726316260509028",
                    "  * PrefabInstance &631051950086114859",
                    "    * GameObject &4890378088392989188 stripped",
                    "      * MonoBehaviour &4749133767824403223",
                    "      * MonoBehaviour &6659483049433300360",
                    "    * Transform &4836536120345214093 stripped",
                    "  * PrefabInstance &8917471562729173351",
                    "    * GameObject &3521489118299776328 stripped",
                    "      * MonoBehaviour &9068813882046058031",
                    "      * MonoBehaviour &2152962228995310295",
                    "    * Transform &3467235967246025665 stripped",
                }
            }
        )]
        [DataRow(
            "Resources/ReorderedComponentsA.prefab",
            new object[]
            {
                new[]
                {
                    "* ReorderedComponents &2853174732634746890",
                    "  * Transform &6356941340319174409",
                    "  * MonoBehaviour &6968487965431459191",
                    "  * MonoBehaviour &4797569303984820519",
                }
            }
        )]
        [DataRow(
            "Resources/ReorderedComponentsAReordered.prefab",
            new object[]
            {
                new[]
                {
                    "* ReorderedComponents &2853174732634746890",
                    "  * Transform &6356941340319174409",
                    "  * MonoBehaviour &4797569303984820519",
                    "  * MonoBehaviour &6968487965431459191",
                }
            }
        )]
        [DataRow(
            "Resources/ReorderedComponentsB.prefab",
            new object[]
            {
                new[]
                {
                    "* ReorderedComponents &7097020843408076707",
                    "  * Transform &2151957425077981344",
                    "  * MonoBehaviour &536295912774078094",
                    "  * MonoBehaviour &2688692364277560030",
                }
            }
        )]
        [DataRow(
            "Resources/ReorderedComponentsBReordered.prefab",
            new object[]
            {
                new[]
                {
                    "* ReorderedComponents &7097020843408076707",
                    "  * Transform &2151957425077981344",
                    "  * MonoBehaviour &2688692364277560030",
                    "  * MonoBehaviour &536295912774078094",
                }
            }
        )]
        [DataRow(
            "Resources/Avatars/NeosAvatar_SetupVRC_Arktoon.prefab",
            new object[]
            {
                new[]
                {
                    "* NeosAvatar_SetupVRC_Arktoon &1485637429502818",
                    "  * Transform &4090137802650142",
                    "  * Animator &95669306525475128",
                    "  * MonoBehaviour &114475674719334502",
                    "  * MonoBehaviour &114654530050479018",
                    "  * Armature &1673165650006850",
                    "    * Transform &4274638513510230",
                    "    * Hips &1836999720211328",
                    "      * Transform &4299522366458874",
                    "      * Leg_L &1085719961373852",
                    "        * Transform &4628790506791126",
                    "        * Knee_L &1919435193852334",
                    "          * Transform &4944953493119748",
                    "          * Ankle_L &1049526202608562",
                    "            * Transform &4470046488242808",
                    "            * Toe_L &1670780399506718",
                    "              * Transform &4236280356479658",
                    "              * Toe_L_end &1943825437005386",
                    "                * Transform &4976476064118952",
                    "      * Leg_R &1103673152876712",
                    "        * Transform &4486716586596050",
                    "        * Knee_R &1981834303621454",
                    "          * Transform &4665586338736186",
                    "          * Ankle_R &1353474294907800",
                    "            * Transform &4444804570577496",
                    "            * Toe_R &1426534221173376",
                    "              * Transform &4992744754938072",
                    "              * Toe_R_end &1404820867214996",
                    "                * Transform &4987597691352012",
                    "      * Spine &1528460475110928",
                    "        * Transform &4676684237267372",
                    "        * Chest &1843065859045294",
                    "          * Transform &4963041818330594",
                    "          * Neck &1850784522645436",
                    "            * Transform &4581894541441626",
                    "            * DB_Col &1856054423902376",
                    "              * Transform &4674907925407494",
                    "              * MonoBehaviour &114077478578563232",
                    "            * Head &1637292357291346",
                    "              * Transform &4694160535342390",
                    "              * MonoBehaviour &114342156744278180",
                    "              * MonoBehaviour &114128526469173048",
                    "              * MonoBehaviour &114955585247182394",
                    "              * MonoBehaviour &114395862211133384",
                    "              * MonoBehaviour &114387924956236892",
                    "              * MonoBehaviour &114246971932492026",
                    "              * MonoBehaviour &114107571372966826",
                    "              * MonoBehaviour &114091417087742022",
                    "              * MonoBehaviour &114152579408271256",
                    "              * MonoBehaviour &114182072647160364",
                    "              * MonoBehaviour &114157346879508026",
                    "              * MonoBehaviour &114927804502517304",
                    "              * Ear_L &1459888722083756",
                    "                * Transform &4357584052819180",
                    "                * MonoBehaviour &114175741090206750",
                    "                * Ear_L.002 &1072537915736500",
                    "                  * Transform &4017377272235382",
                    "                  * Ear_L.001 &1173240831498296",
                    "                    * Transform &4906700548803922",
                    "                    * Ear_L.001_end &1503560211389982",
                    "                      * Transform &4693293812875930",
                    "              * Ear_R &1673993321489460",
                    "                * Transform &4773938504246964",
                    "                * MonoBehaviour &114558366044872626",
                    "                * Ear_R.002 &1609027410559514",
                    "                  * Transform &4412515924433132",
                    "                  * Ear_R.001 &1991092437756556",
                    "                    * Transform &4386313731593096",
                    "                    * Ear_R.001_end &1254295161221576",
                    "                      * Transform &4968827787254744",
                    "              * Eye_L &1976043738401518",
                    "                * Transform &4718967079569378",
                    "                * Eye_L_end &1264330200470776",
                    "                  * Transform &4779948024005016",
                    "              * Eye_R &1464079621850436",
                    "                * Transform &4115831284113068",
                    "                * Eye_R_end &1595961172273130",
                    "                  * Transform &4987529224693316",
                    "              * Hair1_L &1364193427015646",
                    "                * Transform &4708361840199380",
                    "                * Hair1_L.001 &1919721858783468",
                    "                  * Transform &4632348256851664",
                    "                  * Hair1_L.002 &1878743961151412",
                    "                    * Transform &4536776605943028",
                    "                    * Hair1_L.002_end &1403233129153274",
                    "                      * Transform &4283101463563228",
                    "              * Hair1_R &1272390185774664",
                    "                * Transform &4402221595259240",
                    "                * Hair1_R.001 &1134301965077522",
                    "                  * Transform &4815192152155384",
                    "                  * Hair1_R.002 &1063169555112510",
                    "                    * Transform &4789326080971692",
                    "                    * Hair1_R.002_end &1226372872860024",
                    "                      * Transform &4002240520149098",
                    "              * Hair2_L &1651343366340390",
                    "                * Transform &4907372299809982",
                    "                * Hair2_L.001 &1391684103329284",
                    "                  * Transform &4954740703737398",
                    "                  * Hair2_L.002 &1446729512337766",
                    "                    * Transform &4228444038839088",
                    "                    * Hair2_L.002_end &1877271162759428",
                    "                      * Transform &4849975780282774",
                    "              * Hair2_R &1845733171585130",
                    "                * Transform &4360467483517672",
                    "                * Hair2_R.001 &1285185810898534",
                    "                  * Transform &4534929363995826",
                    "                  * Hair2_R.002 &1785010327452584",
                    "                    * Transform &4268412314059140",
                    "                    * Hair2_R.002_end &1189749485525682",
                    "                      * Transform &4558429862299994",
                    "              * Hair3_L &1734099794098682",
                    "                * Transform &4473757523872692",
                    "                * Hair3_L.001 &1007614713703898",
                    "                  * Transform &4162121457736068",
                    "                  * Hair3_L.002 &1762223453175138",
                    "                    * Transform &4405469472238818",
                    "                    * Hair3_L.002_end &1706935713770968",
                    "                      * Transform &4199777115838442",
                    "              * Hair3_R &1205263818845022",
                    "                * Transform &4684112764712960",
                    "                * Hair3_R.001 &1439808230027024",
                    "                  * Transform &4105355253460686",
                    "                  * Hair3_R.002 &1804150588459172",
                    "                    * Transform &4296285923232300",
                    "                    * Hair3_R.002_end &1049183383464654",
                    "                      * Transform &4301149134034192",
                    "              * Hair4_L &1271434712390304",
                    "                * Transform &4038363348652500",
                    "                * Hair4_L.001 &1252316710057816",
                    "                  * Transform &4513664370377112",
                    "                  * Hair4_L.002 &1281967300942440",
                    "                    * Transform &4894754087446824",
                    "                    * Hair4_L.003 &1118144564336206",
                    "                      * Transform &4281106958420012",
                    "                      * Hair4_L.003_end &1350285126374692",
                    "                        * Transform &4475728770603638",
                    "              * Hair4_R &1743766516136780",
                    "                * Transform &4550835779389170",
                    "                * Hair4_R.001 &1131123188437694",
                    "                  * Transform &4833035528245380",
                    "                  * Hair4_R.002 &1490797491854408",
                    "                    * Transform &4917343145900424",
                    "                    * Hair4_R.003 &1405205822882730",
                    "                      * Transform &4995177111050728",
                    "                      * Hair4_R.003_end &1181108258015240",
                    "                        * Transform &4348791925967054",
                    "              * Hair5_L &1184362110533862",
                    "                * Transform &4874848624638438",
                    "                * Hair5_L.001 &1051388557039320",
                    "                  * Transform &4549496464231560",
                    "                  * Hair5_L.002 &1478634790573786",
                    "                    * Transform &4431168850435412",
                    "                    * Hair5_L.003 &1147335530101216",
                    "                      * Transform &4896921504791870",
                    "                      * Hair5_L.003_end &1164062620689180",
                    "                        * Transform &4079624949539278",
                    "              * Hair5_R &1716772723208868",
                    "                * Transform &4689227443718218",
                    "                * Hair5_R.001 &1510151301310904",
                    "                  * Transform &4500671763519688",
                    "                  * Hair5_R.002 &1879697643861718",
                    "                    * Transform &4247234106502568",
                    "                    * Hair5_R.003 &1956476676295046",
                    "                      * Transform &4286973927047924",
                    "                      * Hair5_R.003_end &1711562951875466",
                    "                        * Transform &4050246250149750",
                    "              * Hair6 &1468336173278668",
                    "                * Transform &4337236064828428",
                    "                * Hair6.001 &1671166950634444",
                    "                  * Transform &4055546316752310",
                    "                  * Hair6.002 &1008108804563134",
                    "                    * Transform &4613026055928076",
                    "                    * Hair6.003 &1345656132037520",
                    "                      * Transform &4422103973281666",
                    "                      * Hair6.004 &1392976724167346",
                    "                        * Transform &4982259539333290",
                    "                        * Hair6.004_end &1734549394267766",
                    "                          * Transform &4963066395350052",
                    "              * Hair7 &1622143991560816",
                    "                * Transform &4531128317862800",
                    "                * Hair7.001 &1592703043809334",
                    "                  * Transform &4151734629675158",
                    "                  * Hair7.002 &1706330673941564",
                    "                    * Transform &4249421824033712",
                    "                    * Hair7.003 &1840437140388484",
                    "                      * Transform &4601528108783630",
                    "                      * Hair7.004 &1573458917510764",
                    "                        * Transform &4909281424177646",
                    "                        * Hair7.004_end &1983355981804054",
                    "                          * Transform &4727319026235904",
                    "          * Shoulder_L &1127784982026848",
                    "            * Transform &4514710831667046",
                    "            * Arm_L &1798918757415050",
                    "              * Transform &4325894741230128",
                    "              * Elbow_L &1933861148152760",
                    "                * Transform &4825026192107618",
                    "                * Wrist_L &1672787915225218",
                    "                  * Transform &4265150065327620",
                    "                  * Index_Proximal_L &1692872543962854",
                    "                    * Transform &4693590177846630",
                    "                    * Index_Intermediate_L &1964907214990334",
                    "                      * Transform &4780343277410128",
                    "                      * Index_Distal_L &1022310971982052",
                    "                        * Transform &4243912973777694",
                    "                        * Index_Distal_L_end &1486319555255022",
                    "                          * Transform &4864212214231288",
                    "                  * Little_Proximal_L &1507734252498924",
                    "                    * Transform &4197355216333410",
                    "                    * Little_Intermediate_L &1222311412220652",
                    "                      * Transform &4215411033462388",
                    "                      * Little_Distal_L &1688840213884034",
                    "                        * Transform &4801758614034556",
                    "                        * Little_Distal_L_end &1381572492068370",
                    "                          * Transform &4604976001257164",
                    "                  * Middle_Proximal_L &1958296527753588",
                    "                    * Transform &4096049973070418",
                    "                    * Middle_Intermediate_L &1868493998848088",
                    "                      * Transform &4723490705578310",
                    "                      * Middle_Distal_L &1849212995583216",
                    "                        * Transform &4306204229776638",
                    "                        * Middle_Distal_L_end &1678662374568796",
                    "                          * Transform &4025612777570020",
                    "                  * Ring_Proximal_L &1731933555264230",
                    "                    * Transform &4713596956129694",
                    "                    * Ring_Intermediate_L &1632835546128964",
                    "                      * Transform &4126530235611576",
                    "                      * Ring_Distal_L &1344874999921508",
                    "                        * Transform &4988829474008914",
                    "                        * Ring_Distal_L_end &1544071108789360",
                    "                          * Transform &4325286866991970",
                    "                  * Thumb_Proximal_L &1640001104360504",
                    "                    * Transform &4502326488391080",
                    "                    * Thumb_Intermediate_L &1868479865743064",
                    "                      * Transform &4742848036451358",
                    "                      * Thumb_Distal_L &1146317678441680",
                    "                        * Transform &4007628732761594",
                    "                        * Thumb_Distal_L_end &1141030067525348",
                    "                          * Transform &4556289343588688",
                    "          * Shoulder_R &1007955428600224",
                    "            * Transform &4030824431772584",
                    "            * Arm_R &1466153221366926",
                    "              * Transform &4113666029907648",
                    "              * Elbow_R &1885687115079398",
                    "                * Transform &4993163989388608",
                    "                * Wrist_R &1389261558520596",
                    "                  * Transform &4359103875412324",
                    "                  * Index_Proximal_R &1738469095534566",
                    "                    * Transform &4433919064945830",
                    "                    * Index_Intermediate_R &1724764071908898",
                    "                      * Transform &4576527152152034",
                    "                      * Index_Distal_R &1731804057697460",
                    "                        * Transform &4911462133754560",
                    "                        * Index_Distal_R_end &1616785332910126",
                    "                          * Transform &4108457858448996",
                    "                  * Little_Proximal_R &1823500896738544",
                    "                    * Transform &4156348728484728",
                    "                    * Little_Intermediate_R &1830528680730566",
                    "                      * Transform &4024410900325230",
                    "                      * Little_Distal_R &1774434726131486",
                    "                        * Transform &4736300620418602",
                    "                        * Little_Distal_R_end &1459695923743662",
                    "                          * Transform &4007403068845874",
                    "                  * Middle_Proximal_R &1974696804784126",
                    "                    * Transform &4005106463659978",
                    "                    * Middle_Intermediate_R &1973241891354078",
                    "                      * Transform &4380168405643710",
                    "                      * Middle_Distal_R &1604229376064538",
                    "                        * Transform &4324490359951326",
                    "                        * Middle_Distal_R_end &1475484762115256",
                    "                          * Transform &4617867571384562",
                    "                  * Ring_Proximal_R &1531375986852054",
                    "                    * Transform &4017356126675998",
                    "                    * Ring_Intermediate_R &1403246548540378",
                    "                      * Transform &4217200731454512",
                    "                      * Ring_Distal_R &1525106198437742",
                    "                        * Transform &4545533554971408",
                    "                        * Ring_Distal_R_end &1815074778890078",
                    "                          * Transform &4898455650108880",
                    "                  * Thumb_Proximal_R &1628697781371282",
                    "                    * Transform &4562708262639248",
                    "                    * Thumb_Intermediate_R &1651756519908808",
                    "                      * Transform &4088877548320908",
                    "                      * Thumb_Distal_R &1334656533528752",
                    "                        * Transform &4557869908115744",
                    "                        * Thumb_Distal_R_end &1539435558511850",
                    "                          * Transform &4714882019750360",
                    "          * Aniti-G_System &1973944949648092",
                    "            * Transform &4770037759829642",
                    "            * MeshFilter &33155593486975656",
                    "            * MeshRenderer &23416957466465658",
                    "      * Tail &1086597935773644",
                    "        * Transform &4685862288105592",
                    "        * MonoBehaviour &114635326103303292",
                    "        * Tail.001 &1024297522237802",
                    "          * Transform &4914488221533780",
                    "          * Tail.002 &1093332497651656",
                    "            * Transform &4946788143054512",
                    "            * Tail.003 &1020133200844662",
                    "              * Transform &4273830039599548",
                    "              * Tail.004 &1121838810155282",
                    "                * Transform &4886292480602932",
                    "                * Tail.005 &1642771324176720",
                    "                  * Transform &4788099580967824",
                    "                  * Tail.006 &1293348074922032",
                    "                    * Transform &4910993717795390",
                    "                    * Tail.007 &1363272386058346",
                    "                      * Transform &4535612974094456",
                    "                      * Tail.007_end &1803214405069486",
                    "                        * Transform &4199662094494412",
                    "  * Body &1973186599537414",
                    "    * Transform &4270680045013796",
                    "    * SkinnedMeshRenderer &137668595604221610",
                }
            }
        )]
        [DataRow(
            "Resources/Avatars/Toastacuga.prefab",
            new object[]
            {
                new[]
                {
                    "* Prefab &100100000",
                    "* Toastacuga &1445245993874940",
                    "  * Transform &4851560997859476",
                    "  * Animator &95205787104890620",
                    "  * MonoBehaviour &114836506822813342",
                    "  * MonoBehaviour &114058558008737302",
                    "  * MonoBehaviour &114599451597683412",
                    "  * MonoBehaviour &114807787861604502",
                    "  * MonoBehaviour &114045603309316844",
                    "  * Armature &1088564375853744",
                    "    * Transform &4495265534406746",
                    "    * Hips &1597624641598878",
                    "      * Transform &4117231113678480",
                    "      * Spine &1089812430517284",
                    "        * Transform &4228478796503170",
                    "        * Chest &1917489308499132",
                    "          * Transform &4338100572381728",
                    "          * Neck &1946863680175726",
                    "            * Transform &4029194615988564",
                    "            * Head &1275203178750788",
                    "              * Transform &4161216210117954",
                    "              * Ear01_L &1991901514065850",
                    "                * Transform &4435756820064776",
                    "                * Ear02_L &1688117993743752",
                    "                  * Transform &4066472589302156",
                    "                  * Ear03_L &1024189872583020",
                    "                    * Transform &4991922348054010",
                    "                    * EarEnd_L &1235614415501958",
                    "                      * Transform &4732168790739388",
                    "              * Ear01_R &1424905299069572",
                    "                * Transform &4493555135383324",
                    "                * Ear02_R &1590362331515904",
                    "                  * Transform &4428977623335526",
                    "                  * Ear03_R &1575969618612196",
                    "                    * Transform &4621059774963156",
                    "                    * EarEnd_R &1901035337259242",
                    "                      * Transform &4793365986826468",
                    "              * Eye_L &1088860036501212",
                    "                * Transform &4318710525293562",
                    "                * EyeTrail1 &1433886166281246",
                    "                  * Transform &4562957614052926",
                    "                  * TrailRenderer &96249409053858956",
                    "              * Eye_R &1524436835200348",
                    "                * Transform &4068630432354480",
                    "                * EyeTrail1 &1405613024390402",
                    "                  * Transform &4145686232461366",
                    "                  * TrailRenderer &96961919676496702",
                    "              * Jaw &1211262628561410",
                    "                * Transform &4021710443610450",
                    "              * LeftEye &1297557916362578",
                    "                * Transform &4311180523309230",
                    "              * RightEye &1650374744158586",
                    "                * Transform &4826194088117486",
                    "          * Shoulder_L &1083707720117728",
                    "            * Transform &4749034375279818",
                    "            * UpperArm_L &1976975013989496",
                    "              * Transform &4481097087065222",
                    "              * ForeArm_L &1363142904678776",
                    "                * Transform &4593919394288678",
                    "                * Hand_L &1425621168426258",
                    "                  * Transform &4952416781434702",
                    "                  * MonoBehaviour &114220108240807710",
                    "                  * Index01_L &1629093178354636",
                    "                    * Transform &4220462206613292",
                    "                    * Index02_L &1559109732711114",
                    "                      * Transform &4632093470940384",
                    "                      * Index03_L &1903175605293832",
                    "                        * Transform &4277324383939594",
                    "                  * Middle01_L &1402292861159170",
                    "                    * Transform &4231592053423174",
                    "                    * Middle02_L &1230780542180060",
                    "                      * Transform &4618394225531176",
                    "                      * Middle03_L &1780986644796060",
                    "                        * Transform &4286556355189826",
                    "                  * Pinky01_L &1335438415198770",
                    "                    * Transform &4876862271214648",
                    "                    * Pinky02_L &1984671128157492",
                    "                      * Transform &4375298513209196",
                    "                      * Pinky03_L &1793318782591570",
                    "                        * Transform &4554787833962388",
                    "                  * Thumb01_L &1482061574078866",
                    "                    * Transform &4997610656378490",
                    "                    * Thumb02_L &1779027530101464",
                    "                      * Transform &4195318319141460",
                    "                      * Thumb03_L &1433643949170794",
                    "                        * Transform &4626306452998128",
                    "                * ArmWing01_L &1750384411135920",
                    "                  * Transform &4325560029550968",
                    "                  * ArmWing02_L &1636314110172800",
                    "                    * Transform &4129811682584842",
                    "                * WristWing01_L &1045315637870514",
                    "                  * Transform &4197537267395322",
                    "                  * WristWing02_L &1147133203154444",
                    "                    * Transform &4389035703316174",
                    "          * Shoulder_R &1932869057378338",
                    "            * Transform &4707429837844348",
                    "            * UpperArm_R &1954511785529472",
                    "              * Transform &4738596246695532",
                    "              * ForeArm_R &1951953325583080",
                    "                * Transform &4431045924734304",
                    "                * Hand_R &1777670886814692",
                    "                  * Transform &4384877633955140",
                    "                  * MonoBehaviour &114981158884828032",
                    "                  * Index01_R &1778200505095650",
                    "                    * Transform &4502961865050560",
                    "                    * Index02_R &1193254972343148",
                    "                      * Transform &4824369831206426",
                    "                      * Index03_R &1238322395000236",
                    "                        * Transform &4742462677825454",
                    "                  * Middle01_R &1367887303352612",
                    "                    * Transform &4226864365437128",
                    "                    * Middle02_R &1061880032353220",
                    "                      * Transform &4867992338134736",
                    "                      * Middle03_R &1579164215450622",
                    "                        * Transform &4911163392100688",
                    "                  * Pinky01_R &1883249758908534",
                    "                    * Transform &4855181451688064",
                    "                    * Pinky02_R &1270845207518294",
                    "                      * Transform &4105008763204190",
                    "                      * Pinky03_R &1183556746499162",
                    "                        * Transform &4301592069963452",
                    "                  * Thumb01_R &1938048948361120",
                    "                    * Transform &4284237375533422",
                    "                    * Thumb02_R &1911078293905584",
                    "                      * Transform &4789933850298618",
                    "                      * Thumb03_R &1970419024157462",
                    "                        * Transform &4106318294858650",
                    "                * ArmWing01_R &1683616174378098",
                    "                  * Transform &4292324585022052",
                    "                  * ArmWing02_R &1718385035372594",
                    "                    * Transform &4833502688538468",
                    "                * WristWing01_R &1558143689606120",
                    "                  * Transform &4763399350125314",
                    "                  * WristWing02_R &1921677071232228",
                    "                    * Transform &4919851431692678",
                    "          * Signature &1504095064935422",
                    "            * Transform &4536329425213638",
                    "            * MeshFilter &33036375853242462",
                    "            * MeshRenderer &23539984594207520",
                    "      * TailRoot &1296122545128014",
                    "        * Transform &4376235583057004",
                    "        * Tail01 &1158689688025664",
                    "          * Transform &4937803703217578",
                    "          * Tail02 &1857885809456268",
                    "            * Transform &4399018099436864",
                    "            * Tail03 &1059621996208584",
                    "              * Transform &4091297180021436",
                    "              * Tail04 &1500659481604412",
                    "                * Transform &4677615797925822",
                    "                * TailEnd &1137074068722980",
                    "                  * Transform &4079498741638732",
                    "                  * TailEnd_end &1985919615498396",
                    "                    * Transform &4055992163666628",
                    "      * UpperLeg_L &1460258748323908",
                    "        * Transform &4832197507330286",
                    "        * LowerLeg_L &1771504349541720",
                    "          * Transform &4440322325597302",
                    "          * Foot_L &1843613438336722",
                    "            * Transform &4518172264178860",
                    "            * Toes_L &1502572320754908",
                    "              * Transform &4417019006816346",
                    "      * UpperLeg_R &1708808934445754",
                    "        * Transform &4374956489041322",
                    "        * LowerLeg_R &1167959699388406",
                    "          * Transform &4031129302882194",
                    "          * Foot_R &1749476311689996",
                    "            * Transform &4603214812059182",
                    "            * Toes_R &1016518595886042",
                    "              * Transform &4973057799004732",
                    "  * Body &1338271184007530",
                    "    * Transform &4321763930331906",
                    "    * SkinnedMeshRenderer &137715300468256256",
                }
            }
        )]
        public void SelfDiff(string path, string[] expectedLines)
        {
            this.Diff(path, path, expectedLines, 0);
        }

        /// <summary>
        ///     Tests the diff of two prefab files.
        /// </summary>
        /// <param name="leftPath">The path to the left prefab file.</param>
        /// <param name="rightPath">The path to the right prefab file.</param>
        /// <param name="expectedLines">
        ///     The expected diff output, excluding trailing line breaks.
        /// </param>
        [DataTestMethod]
        [DataRow(
            "Resources/NestedPrefab3.prefab",
            "Resources/NestedPrefab3a.prefab",
            new[]
            {
                "* Nested Prefab 3 &4989725120814738964 -> &6293954053335516682",
                "  * Transform &8820928727568499859 -> &7497849768378543245",
                "  * PrefabInstance &384786867856306869 -> &1670981476826454699",
                "    * Transform &9168539870313657894 -> &7879820769096490552 stripped",
                "  * PrefabInstance &476074792185059992 -> &1512319774972293766",
                "    * Transform &9003083168696704523 -> &7968817328979179029 stripped",
                "    * Transform &2786369713623583212 -> &3804593009358545394 stripped",
                "      * New Inner Child &5073144092867333913 -> &6053085283548292871",
                "        * Transform &1912756242714561364 -> &643158201123562314",
            },
            1
        )]
        [DataRow(
            "Resources/NestedPrefab2.prefab",
            "Resources/NestedPrefab3a.prefab",
            new[]
            {
                "* Nested Prefab 2 &4989725120814738964 -> Nested Prefab 3 &6293954053335516682",
                "  * Transform &8820928727568499859 -> &7497849768378543245",
                "  * PrefabInstance &4156974465077255250 -> &1670981476826454699",
                "    * Transform &4889415719643437249 -> &7879820769096490552 stripped",
                "  * PrefabInstance &4431041678912474380 -> &1512319774972293766",
                "    * Transform &5121722109705652639 -> &7968817328979179029 stripped",
                "    > Transform &3804593009358545394 stripped",
                "      > New Inner Child &6053085283548292871",
                "        > Transform &643158201123562314",
            },
            1
        )]
        [DataRow(
            "Resources/GameObjectAndPrefabInstanceComponentsA.prefab",
            "Resources/GameObjectAndPrefabInstanceComponentsB.prefab",
            new[]
            {
                "* GameObjectAndPrefabInstanceComponents &1295435386291451790 -> &3703353320363238006",
                "  * Transform &6900024346194483700 -> &9034067136953502732",
                "  * Child 1 &3246267058468154075 -> &1122439843563388707",
                "    * Transform &446762813002330973 -> &2642913480352949925",
                "    * MonoBehaviour &2445294676174449731 -> &248213834871142843",
                "    * MonoBehaviour &8101453579712206767 -> &5976451239843250775",
                "  * Child 2 &1130121813463608664 -> &3256177755652880544",
                "    * Transform &9040806159793680495 -> &6911300203454047639",
                "    * MonoBehaviour &1527149206699838427 -> &4012493667704102435",
                "    * MonoBehaviour &3122549251109195932 -> &705726316260509028",
                "  * PrefabInstance &3053240162318261203 -> &631051950086114859",
                "    * GameObject &7007784988793393148 -> &4890378088392989188 stripped",
                "      * MonoBehaviour &7167184517858377455 -> &4749133767824403223",
                "      * MonoBehaviour &9148366837940496496 -> &6659483049433300360",
                "    * Transform &7025730173712034165 -> &4836536120345214093 stripped",
                "  * PrefabInstance &6440009496554267807 -> &8917471562729173351",
                "    * GameObject &1315169930923323568 -> &3521489118299776328 stripped",
                "      * MonoBehaviour &6864714582965902295 -> &9068813882046058031",
                "      * MonoBehaviour &4575199936328865583 -> &2152962228995310295",
                "    * Transform &1333244823837382201 -> &3467235967246025665 stripped",
            },
            1
        )]
        [DataRow(
            "Resources/ReorderedComponentsA.prefab",
            "Resources/ReorderedComponentsB.prefab",
            new[]
            {
                "* ReorderedComponents &2853174732634746890 -> &7097020843408076707",
                "  * Transform &6356941340319174409 -> &2151957425077981344",
                "  * MonoBehaviour &6968487965431459191 -> &2688692364277560030",
                "  * MonoBehaviour &4797569303984820519 -> &536295912774078094",
            },
            1
        )]
        [DataRow(
            "Resources/ReorderedComponentsAReordered.prefab",
            "Resources/ReorderedComponentsBReordered.prefab",
            new[]
            {
                "* ReorderedComponents &2853174732634746890 -> &7097020843408076707",
                "  * Transform &6356941340319174409 -> &2151957425077981344",
                "  * MonoBehaviour &4797569303984820519 -> &536295912774078094",
                "  * MonoBehaviour &6968487965431459191 -> &2688692364277560030",
            },
            1
        )]
        public void Diff(
            string leftPath,
            string rightPath,
            string[] expectedLines,
            int expectedExitCode
        )
        {
            TestConsole testConsole = new();
            int exitCode = Program.RootCommand.Invoke(
                new[] { leftPath, rightPath },
                testConsole
            );

            Assert.AreEqual(
                string.Join("", expectedLines.Select(line => line + Environment.NewLine)),
                testConsole.Out.ToString()
            );
            Assert.AreEqual("", testConsole.Error.ToString());
            Assert.AreEqual(expectedExitCode, exitCode);
        }
    }
}
