using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace JobResultControl
{
    public class ResultReport
    {
        #region Variables

        // log
        LogFile.Log log;

        #endregion

        // Constructor
        public ResultReport()
        {
            // load configures
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            {
                /* ----------------------------------------------
                 * Log file.
                 * ----------------------------------------------*/
                string logPath;
                try
                {
                    logPath = cm.GetCharValue("VIS_LogPath");
                    if (logPath.Equals("")) logPath = @"C:\BHM\Log\VISION";
                }
                catch { logPath = @"C:\BHM\Log\VISION"; }

                /* ----------------------------------------------
                 * Create log file
                 * ----------------------------------------------*/
                log = new LogFile.Log(logPath, "vision");
                log.AppendText("===============================================");
            }
        }

        public int Report(string table, string job, string score1, string score2, string score3, string score4, string runStatus)
        {
            try
            {
                log.AppendText(string.Format("Report() ปรับปรุ่งข้อมูล {0}.{1} : {2},{3},{4},{5},{6}", table, job, score1, score2, score3, score4, runStatus));
                int result = 1;

                // อ่าน VIS_LastTotalMinutes เขียนฐานข้อมูล
                using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
                {
                    using (DB_Manager.VIS_Management vis = new DB_Manager.VIS_Management())
                    {
                        // อ่านค่าเลขรายการที่ต้องบันทึก
                        // ออกโดย kiosk
                        long lastTotalMinutes = cm.GetIntValue("VIS_LAST_VIS_ID");
                        int lastStateID = (int)cm.GetIntValue("VIS_LastState_ID");

                        // บันทึก
                        log.AppendText(string.Format("Records -> {0}, {1}", lastTotalMinutes, lastStateID));

                        // เพิ่มค่า state id
                        lastStateID++;
                        if (0 == cm.SetIntValue("VIS_LastState_ID", lastStateID, lastStateID.ToString())) log.AppendText("ERROR! Report() แก้ไข VIS_LastState_ID ไม่ได้");

                        // ลงรายงาน
                        if (0 < lastTotalMinutes)
                        {
                            // validation
                            float sc1, sc2, sc3, sc4;
                            if (!float.TryParse(score1, out sc1)) sc1 = -1;
                            if (!float.TryParse(score2, out sc2)) sc2 = -1;
                            if (!float.TryParse(score3, out sc3)) sc3 = -1;
                            if (!float.TryParse(score4, out sc4)) sc4 = -1;

                            // เขียนรายงาน
                            if (0 < vis.Update(lastTotalMinutes, lastStateID, job, sc1, sc2, sc3, sc4, runStatus))
                            {
                                // LOG
                                log.AppendText("Report() สำเร็จ");
                            }
                            else
                            {
                                // log
                                log.AppendText("ERROR! Report() ไม่มีข้อมูลถูกปรับปรุง อาจเป็นเพราะยังไม่มี Journal");
                            }
                        }
                        else
                        {
                            // เขียนบันทึก ผิดพลาด
                            log.AppendText("ERROR! Report() ไม่พบ VIS_LastTotalMinutes");
                        }
                    }
                }

                // จบ
                return result;
            }
            catch (Exception ex) { log.AppendText("ERROR! Report() ไปยัง ฐานข้อมูล ผิดพลาด " + ex.Message); throw ex; }
        }

        public struct TwoToneModel
        {
            public int Count;
            public string AnalysisDescription;
            public bool NO_GOOD;

            private long _vis_id;
            private bool _isFront;

            public TwoToneModel(long vis_id, bool isFront)
            {
                Count = 0;
                AnalysisDescription = "Initialized";
                NO_GOOD = false;
                this._vis_id = vis_id;
                this._isFront = isFront;
                UpdateMode();
            }

            public void UpdateMode()
            {
                if (_isFront) UpdateModel_TTF(); // TTF
                else UpdateModel_TTS(); // TTS
            }

            public void UpdateModel_TTF()
            {
                using (DB_Manager.VIS_Management vis = new DB_Manager.VIS_Management())
                {
                    // ข้อมูล
                    DataTable dt = vis.Select(this._vis_id);

                    // ram
                    decimal[,] A_Score = new decimal[10, 4];
                    string[] A_RunStatus = new string[10];
                    decimal[,] B_Score = new decimal[10, 4];
                    string[] B_RunStatus = new string[10];

                    // update ram
                    try
                    {
                        for (int idxt = 0; idxt < 10; idxt++)
                        {
                            A_Score[idxt, 0] = (decimal)(dt.Rows[0][string.Format("A{0:00}_SCORE1", idxt + 1)]);
                            A_Score[idxt, 1] = (decimal)(dt.Rows[0][string.Format("A{0:00}_SCORE2", idxt + 1)]);
                            A_Score[idxt, 2] = (decimal)(dt.Rows[0][string.Format("A{0:00}_SCORE3", idxt + 1)]);
                            A_Score[idxt, 3] = (decimal)(dt.Rows[0][string.Format("A{0:00}_SCORE4", idxt + 1)]);
                            A_RunStatus[idxt] = (string)(dt.Rows[0][string.Format("A{0:00}_RUN_STATUS", idxt + 1)]);
                        }
                        for (int idxt = 0; idxt < 10; idxt++)
                        {
                            B_Score[idxt, 0] = (decimal)(dt.Rows[0][string.Format("B{0:00}_SCORE1", idxt + 1)]);
                            B_Score[idxt, 1] = (decimal)(dt.Rows[0][string.Format("B{0:00}_SCORE2", idxt + 1)]);
                            B_Score[idxt, 2] = (decimal)(dt.Rows[0][string.Format("B{0:00}_SCORE3", idxt + 1)]);
                            B_Score[idxt, 3] = (decimal)(dt.Rows[0][string.Format("B{0:00}_SCORE4", idxt + 1)]);
                            B_RunStatus[idxt] = (string)(dt.Rows[0][string.Format("B{0:00}_RUN_STATUS", idxt + 1)]);
                        }
                    }
                    catch (Exception ex) { throw ex; }

                    // analysis of first idxt
                    bool isA = false;
                    if (!A_RunStatus[0].ToUpper().Contains("ACCEPT") && !B_RunStatus[0].ToUpper().Contains("ACCEPT"))
                    {
                        // ไม่สามารถระบุชนิด การเข้ามา ของตะกร้าได้ และถือว่า ตรวจจับไม่ได้ ให้เป็น NG
                        AnalysisDescription = "REJECT ใบที่ 1 ไม่สามารถระบุชนิด การเข้ามา ของตะกร้าได้ และถือว่า ตรวจจับไม่ได้ ให้เป็น NG";
                        Count = 0;
                        NO_GOOD = true;
                        return;
                    }
                    else if (A_RunStatus[0].ToUpper().Contains("ACCEPT") && !B_RunStatus[0].ToUpper().Contains("ACCEPT"))
                    {
                        // A type
                        isA = true;
                    }
                    else if (!A_RunStatus[0].ToUpper().Contains("ACCEPT") && B_RunStatus[0].ToUpper().Contains("ACCEPT"))
                    {
                        // B type
                        isA = false;
                    }
                    else if (A_RunStatus[0].ToUpper().Contains("ACCEPT") && B_RunStatus[0].ToUpper().Contains("ACCEPT"))
                    {
                        // ยังระบุไม่ได้ อ่านได้ทั้งสองแบบ
                        AnalysisDescription = "REJECT ใบที่ 1 ยังระบุไม่ได้ ตรวจจับแจ้งว่า วิเคราะห์ผลได้ทั้งสองแบบ";
                        Count = 0;
                        NO_GOOD = true;
                        return;
                    }

                    // analysis for all                    
                    for (int idx = 0; idx < 10; idx++)
                    {
                        // temporary variables
                        decimal sc1, sc2, sc3, sc4;
                        string runStatus;
                        if (isA)
                        {
                            sc1 = A_Score[idx, 0];
                            sc2 = A_Score[idx, 1];
                            sc3 = A_Score[idx, 2];
                            sc4 = A_Score[idx, 3];
                            runStatus = A_RunStatus[idx];
                        }
                        else
                        {
                            sc1 = B_Score[idx, 0];
                            sc2 = B_Score[idx, 1];
                            sc3 = B_Score[idx, 2];
                            sc4 = B_Score[idx, 3];
                            runStatus = B_RunStatus[idx];
                        }

                        // analysis and count
                        if (runStatus.ToUpper().Contains("ACCEPT"))
                        {
                            if (0 < sc1 && 0 < sc2 && 0 < sc3 && 0 < sc4)
                            {
                                // OK
                                AnalysisDescription = string.Format("ACCEPT ใบที่ {0}: {1},{2},{3},{4}", idx + 1, sc1, sc2, sc3, sc4);
                                Count = idx + 1;
                                NO_GOOD = false;
                            }
                            else
                            {
                                // NG
                                AnalysisDescription = string.Format("REJECT ใบที่ {0}: {1},{2},{3},{4}", idx + 1, sc1, sc2, sc3, sc4);
                                NO_GOOD = true;
                                return;
                            }
                        }
                        else
                        {
                            // ตรวจส่วนที่เหลือ เพื่อยืนยันการมีอยู่ของส่วนที่เหลือ
                            switch (idx)
                            {
                                case 0:
                                    break;
                                case 1://เหลือ 2-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[2].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[3].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[2].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[3].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 2://เหลือ 3-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[3].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[3].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 3://เหลือ 4-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 4://เหลือ 5-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 5://เหลือ 6-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 6://เหลือ 7-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 7://เหลือ 8-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 8://เหลือ 9-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 9://ไม่เหลือ
                                    // NG
                                    AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                    Count = idx;
                                    NO_GOOD = false;
                                    return;
                                    break;
                            }                           
                        }

                    }
                }
            }

            public void UpdateModel_TTS()
            {
                using (DB_Manager.VIS_Management vis = new DB_Manager.VIS_Management())
                {
                    // ข้อมูล
                    DataTable dt = vis.Select(this._vis_id);

                    // ram
                    decimal[,] A_Score = new decimal[10, 4];
                    string[] A_RunStatus = new string[10];
                    decimal[,] B_Score = new decimal[10, 4];
                    string[] B_RunStatus = new string[10];

                    // update ram
                    for (int idxt = 0; idxt < 10; idxt++)
                    {
                        A_Score[idxt, 0] = (decimal)(dt.Rows[0][string.Format("A{0:00}_SCORE1", idxt + 1)]);
                        A_Score[idxt, 1] = (decimal)(dt.Rows[0][string.Format("A{0:00}_SCORE2", idxt + 1)]);
                        A_Score[idxt, 2] = (decimal)(dt.Rows[0][string.Format("A{0:00}_SCORE3", idxt + 1)]);
                        A_Score[idxt, 3] = (decimal)(dt.Rows[0][string.Format("A{0:00}_SCORE4", idxt + 1)]);
                        A_RunStatus[idxt] = (string)(dt.Rows[0][string.Format("A{0:00}_RUN_STATUS", idxt + 1)]);
                    }
                    for (int idxt = 0; idxt < 10; idxt++)
                    {
                        B_Score[idxt, 0] = (decimal)(dt.Rows[0][string.Format("B{0:00}_SCORE1", idxt + 1)]);
                        B_Score[idxt, 1] = (decimal)(dt.Rows[0][string.Format("B{0:00}_SCORE2", idxt + 1)]);
                        B_Score[idxt, 2] = (decimal)(dt.Rows[0][string.Format("B{0:00}_SCORE3", idxt + 1)]);
                        B_Score[idxt, 3] = (decimal)(dt.Rows[0][string.Format("B{0:00}_SCORE4", idxt + 1)]);
                        B_RunStatus[idxt] = (string)(dt.Rows[0][string.Format("B{0:00}_RUN_STATUS", idxt + 1)]);
                    }

                    // analysis of first idxt
                    bool isA = false;
                    if (!A_RunStatus[0].ToUpper().Contains("ACCEPT") && !B_RunStatus[0].ToUpper().Contains("ACCEPT"))
                    {
                        // ไม่สามารถระบุชนิด การเข้ามา ของตะกร้าได้ และถือว่า ตรวจจับไม่ได้ ให้เป็น NG
                        AnalysisDescription = "REJECT ใบที่ 1 ไม่สามารถระบุชนิด การเข้ามา ของตะกร้าได้ และถือว่า ตรวจจับไม่ได้ ให้เป็น NG";
                        Count = 0;
                        NO_GOOD = true;
                        return;
                    }
                    else if (A_RunStatus[0].ToUpper().Contains("ACCEPT") && !B_RunStatus[0].ToUpper().Contains("ACCEPT"))
                    {
                        // A type
                        isA = true;
                    }
                    else if (!A_RunStatus[0].ToUpper().Contains("ACCEPT") && B_RunStatus[0].ToUpper().Contains("ACCEPT"))
                    {
                        // B type
                        isA = false;
                    }
                    else if (A_RunStatus[0].ToUpper().Contains("ACCEPT") && B_RunStatus[0].ToUpper().Contains("ACCEPT"))
                    {
                        // ยังระบุไม่ได้ อ่านได้ทั้งสองแบบ
                        AnalysisDescription = "REJECT ใบที่ 1 ยังระบุไม่ได้ ตรวจจับแจ้งว่า วิเคราะห์ผลได้ทั้งสองแบบ";
                        Count = 0;
                        NO_GOOD = true;
                        return;
                    }

                    // analysis for all                    
                    for (int idx = 0; idx < 10; idx++)
                    {
                        // temporary variables
                        decimal sc1, sc2, sc3, sc4;
                        string runStatus;
                        if (isA)
                        {
                            sc1 = A_Score[idx, 0];
                            sc2 = A_Score[idx, 1];
                            sc3 = A_Score[idx, 2];
                            sc4 = A_Score[idx, 3];
                            runStatus = A_RunStatus[idx];
                        }
                        else
                        {
                            sc1 = B_Score[idx, 0];
                            sc2 = B_Score[idx, 1];
                            sc3 = B_Score[idx, 2];
                            sc4 = B_Score[idx, 3];
                            runStatus = B_RunStatus[idx];
                        }

                        // analysis and count
                        if (runStatus.ToUpper().Contains("ACCEPT"))
                        {
                            if (0 < sc1 && 0 < sc2 && 0 < sc3)
                            {
                                // OK
                                AnalysisDescription = string.Format("ACCEPT ใบที่ {0}: {1},{2},{3},{4}", idx + 1, sc1, sc2, sc3, sc4);
                                Count = idx + 1;
                                NO_GOOD = false;
                            }
                            else
                            {
                                // NG
                                AnalysisDescription = string.Format("REJECT ใบที่ {0}: {1},{2},{3},{4}", idx + 1, sc1, sc2, sc3, sc4);
                                NO_GOOD = true;
                                return;
                            }
                        }
                        else
                        {
                            // ตรวจส่วนที่เหลือ เพื่อยืนยันการมีอยู่ของส่วนที่เหลือ
                            switch (idx)
                            {
                                case 0:
                                    break;
                                case 1://เหลือ 2-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[2].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[3].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[2].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[3].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 2://เหลือ 3-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[3].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[3].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 3://เหลือ 4-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[4].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 4://เหลือ 5-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[5].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 5://เหลือ 6-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[6].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 6://เหลือ 7-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[7].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 7://เหลือ 8-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[8].ToUpper().Contains("ACCEPT") &&
                                            !B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 8://เหลือ 9-9
                                    if (isA)
                                    {
                                        // ตรวจที่เหลือ A
                                        if (!A_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        // ตรวจที่เหลือ B
                                        if (!B_RunStatus[9].ToUpper().Contains("ACCEPT"))
                                        {
                                            // OK
                                            AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = false;
                                            return;
                                        }
                                        else
                                        {
                                            // NG
                                            AnalysisDescription = string.Format("REJECT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                            Count = idx;
                                            NO_GOOD = true;
                                            return;
                                        }
                                    }
                                    break;
                                case 9://เหลือ 2-9
                                    // NG
                                    AnalysisDescription = string.Format("ACCEPT ใบที่ {0} : {1}", idx + 1, A_RunStatus[idx]);
                                    Count = idx;
                                    NO_GOOD = false;
                                    return;
                                    break;
                            }
                        }
                    }
                }
            }

            public override string ToString()
            {
                return string.Format("{0}, {1}", Count, AnalysisDescription);
            }
        }

        public long CreateTwoToneFrontJobRecord()
        {
            // สั่งรัน
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            {
                using (DB_Manager.VIS_Management vis = new DB_Manager.VIS_Management())
                {
                    // เรียกค่าเก่า ค่า lastID มากสุด
                    long lastID;
                    try
                    {
                        lastID = vis.GetMaxID();
                    }
                    catch { lastID = 1; }
                    lastID++; // เพิ่มค่า

                    // Journal ID
                    long journalID;
                    string bType;
                    string bFace;
                    try
                    {
                        journalID = cm.GetIntValue("LAST_JOURNAL_ID");
                    }
                    catch { journalID = 0; }
                    try
                    {
                        bType = cm.GetCharValue("LAST_B_TYPE");
                        if (bType.Equals("")) bType = "TEST";
                    }
                    catch { bType = "TEST"; }
                    try
                    {
                        bFace = cm.GetCharValue("LAST_VIS_FACE");
                        if (bFace.Equals("")) bFace = "TEST";
                    }
                    catch { bFace = "TEST"; }


                    // สร้างข้อมูลแถวใหม่
                    int ret = vis.Insert(lastID, journalID, bType, bFace);

                    // สร้างใหม่ได้
                    if (0 < ret)
                    {
                        // เขียนข้อมูลลงใน tbConfig เพื่อพร้อมให้เกิดการวิเคราะห์ด้วยโปรแกรม vpro
                        cm.SetIntValue("VIS_LAST_VIS_ID", lastID, lastID.ToString());
                        cm.SetIntValue("VIS_LastState_ID", 0, "0");
                        cm.SetCharValue("VIS_TTF_RUN_REQUEST", "RUN", "RUN");
                        return lastID;
                    }

                    // สร้างใหม่ไม่ได้ อาจมีอยู่แล้ว
                    else if (0 == ret)
                    {
                        return lastID;
                    }
                }
            }
            return 0;
        }

        public long CreateTwoToneSideJobRecord()
        {
            // สั่งรัน
            using (DB_Manager.ConfigManagement cm = new DB_Manager.ConfigManagement())
            {
                using (DB_Manager.VIS_Management vis = new DB_Manager.VIS_Management())
                {
                    // เรียกค่าเก่า ค่า lastID มากสุด
                    long lastID;
                    try
                    {
                        lastID = vis.GetMaxID();
                    }
                    catch { lastID = 1; }
                    lastID++; // เพิ่มค่า

                    // Journal ID
                    long journalID;
                    string bType;
                    string bFace;
                    try
                    {
                        journalID = cm.GetIntValue("LAST_JOURNAL_ID");
                    }
                    catch { journalID = 0; }
                    try
                    {
                        bType = cm.GetCharValue("LAST_B_TYPE");
                        if (bType.Equals("")) bType = "TEST";
                    }
                    catch { bType = "TEST"; }
                    try
                    {
                        bFace = cm.GetCharValue("LAST_VIS_FACE");
                        if (bFace.Equals("")) bFace = "TEST";
                    }
                    catch { bFace = "TEST"; }


                    // สร้างข้อมูลแถวใหม่
                    int ret = vis.Insert(lastID, journalID, bType, bFace);

                    // สร้างใหม่ได้
                    if (0 < ret)
                    {
                        // เขียนข้อมูลลงใน tbConfig เพื่อพร้อมให้เกิดการวิเคราะห์ด้วยโปรแกรม vpro
                        cm.SetIntValue("VIS_LAST_VIS_ID", lastID, lastID.ToString());
                        cm.SetIntValue("VIS_LastState_ID", 0, "0");
                        cm.SetCharValue("VIS_TTS_RUN_REQUEST", "RUN", "RUN");
                        return lastID;
                    }

                    // สร้างใหม่ไม่ได้ อาจมีอยู่แล้ว
                    else if (0 == ret)
                    {
                        return lastID;
                    }
                }
            }
            return 0;
        }
    }
}
