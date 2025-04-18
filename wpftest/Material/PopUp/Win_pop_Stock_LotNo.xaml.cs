﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WizMes_BooKyong.PopUp
{
    /// <summary>
    /// RheoChoice.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_pop_Stock_LotNo : Window
    {
        int rowNum = 0;

        public string ArticleID = "";
        public string Article = "";
        public string LotID = "";

        public string BuyerArticleNo = "";
        public string ArticleGrp = "";
        public string UnitClssName = "";
        public string StockQty = "";

        

        public string date = "";

        PlusFinder pf = new PlusFinder();

        public Win_mtr_StockControl_U StockControl = new Win_mtr_StockControl_U();

        public List<Win_mtr_StockControl_U_CodeView> lstLotClonePop = new List<Win_mtr_StockControl_U_CodeView>();
        
        

        public Win_pop_Stock_LotNo()
        {
            InitializeComponent();
        }

        public Win_pop_Stock_LotNo(List<Win_mtr_StockControl_U_CodeView> lstLotClonePop)
        {
            InitializeComponent();

            this.lstLotClonePop = lstLotClonePop;
        }

        public Win_pop_Stock_LotNo(string ArticleID, string Article, string LotID, string BuyerArticleNo, string ArticleGrp, string UnitClssName, string StockQty)
        {
            InitializeComponent();

            this.ArticleID = ArticleID;
            this.Article = Article;
            this.LotID = LotID;

            this.BuyerArticleNo = BuyerArticleNo;
            this.ArticleGrp = ArticleGrp;
            this.UnitClssName = UnitClssName;
            this.StockQty = StockQty;

            
        }

        // 콤보박스셋팅
        private void ComboBoxSetting()
        {
            cboArticleGroup.Items.Clear();
            cboWareHouse.Items.Clear();

            ObservableCollection<CodeView> cbArticleGroup = ComboBoxUtil.Instance.Gf_DB_MT_sArticleGrp();
            ObservableCollection<CodeView> cbWareHouse = ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "LOC", "Y", "", "");

            this.cboArticleGroup.ItemsSource = cbArticleGroup;
            this.cboArticleGroup.DisplayMemberPath = "code_name";
            this.cboArticleGroup.SelectedValuePath = "code_id";
            this.cboArticleGroup.SelectedIndex = 0;


            

            this.cboWareHouse.ItemsSource = cbWareHouse;
            this.cboWareHouse.DisplayMemberPath = "code_name";
            this.cboWareHouse.SelectedValuePath = "code_id";
            this.cboWareHouse.SelectedIndex = 0;

        }

        private void MoveSub_Loaded(object sender, RoutedEventArgs e)
        {
            //dtpAdjustDate.SelectedDate = DateTime.Today;

            
            ComboBoxSetting();

            if (!ArticleID.Trim().Equals(""))
            {
                txtArticleSrh.Text = Article;

                //FillGrid_ArticleID(ArticleID);

                return;
            }
            else if (!Article.Trim().Equals(""))
            {
                chkArticle.IsChecked = true;
                txtArticleSrh.Text = Article;
            }

            FillGrid();

            //dtpAdjustDate.SelectedDate = DateTime.Today;
        }

        #region 주요 버튼 이벤트 - 확인, 닫기, 검색

        public List<Win_mtr_StockControl_U_CodeView> lstLotStock = new List<Win_mtr_StockControl_U_CodeView>();

        //확인
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0 ;  i < dgdMain.Items.Count; i++)
            {
                var main = dgdMain.Items[i] as Win_mtr_StockControl_U_CodeView;

                if(main != null && main.Chk == true)
                {
                    lstLotStock.Add(main);

                }

            }

            this.DialogResult = true;
            
        }

        //닫기
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //검색
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            re_Search(rowNum);
        }

        #endregion // 주요 버튼 이벤트


        #region Header 부분 - 검색조건

     
        // 품명
        private void lblArticleSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkArticle.IsChecked == true)
            {
                chkArticle.IsChecked = false;

            }
            else
            {
                chkArticle.IsChecked = true;
            }
        }
        private void chkArticleSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkArticle.IsChecked = true;
            txtArticleSrh.IsEnabled = true;
            btnArticle.IsEnabled = true;
        }
        private void chkArticleSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkArticle.IsChecked = false;
            txtArticleSrh.IsEnabled = false;
            btnArticle.IsEnabled = false;
        }


        #endregion // Header 부분 - 검색조건

    
        #region 주요 메서드 모음

        private void re_Search(int rowNum)
        {
            FillGrid();

            if (dgdMain.Items.Count > 0)
            {
                dgdMain.SelectedIndex = rowNum;
            }
            else
            {
                this.DataContext = null;
                MessageBox.Show("조회된 데이터가 없습니다.");
                return;
            }
        }

        #region 조회

        private void FillGrid()
        {
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("sDate", date);
                sqlParameter.Add("ChkArticle", chkArticle.IsChecked == true ? 1 : 0);
                sqlParameter.Add("ArticleID", chkArticle.IsChecked == true && txtArticleSrh.Tag != null ? txtArticleSrh.Tag.ToString() : "");
                sqlParameter.Add("ArticleGrpID", chkArticleGroup.IsChecked == true && cboArticleGroup.SelectedValue != null ? cboArticleGroup.SelectedValue.ToString() : ""); //제품그룹
                sqlParameter.Add("sToLocID", chkToLocSrh.IsChecked == true ? (cboWareHouse.SelectedValue != null ? cboWareHouse.SelectedValue.ToString() : "") : ""); // 창고
             
                //DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_mtr_StockLotID_WPF", sqlParameter, false);
                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_sbStock_sStockControl_Stock_mtr", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("조회된 데이터가 없습니다.");
                    }
                    else
                    {
                        int index = 0;
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow dr in drc)
                        {
                            index++;
                            var NowStockData = new Win_mtr_StockControl_U_CodeView
                            {
                                Num = index,
                                ArticleID = dr["ArticleID"].ToString(),
                                //LotID = dr["LotID"].ToString(),
                                //LotName = dr["LotID"].ToString(),
                                UnitClss = dr["UnitClss"].ToString(),
                                StuffinQty = dr["StuffinQty"].ToString(),
                                OutQty = dr["Outqty"].ToString(),
                                StockQty = stringFormatN0(dr["StockQty"]),
                                Article = dr["Article"].ToString(),
                                UnitClssName = dr["UnitClssName"].ToString(),
                                BuyerArticleNo = dr["BuyerArticleNo"].ToString(),


                                ArticleGrpID = dr["ArticleGrpID"].ToString(),
                                ArticleGrp = dr["ArticleGrp"].ToString(),
                                TOLocID = dr["TOLocID"].ToString(),
                                ToLocName = dr["ToLocName"].ToString(),
                                LastDate = dr["LastDate"].ToString(),

                                //Color = dr["Color"].ToString(),
                                //Spec = dr["Spec"].ToString(),

                                UDFlag = true,

                            };

                            if (lstLotClonePop.Count > 0)
                            {
                                for (int i = 0; i < lstLotClonePop.Count; i++)
                                {
                                    if (NowStockData.ArticleID.Equals(lstLotClonePop[i].ArticleID.Trim()))
                                    {
                                        NowStockData.StockQty = lstLotClonePop[i].StockQty;
                                    }
                                }
                            }

                            NowStockData.ControlQty = "0";

                            dgdMain.Items.Add(NowStockData);
                        }
                        tblCount.Text = "▶검색개수 : " + index + "건";

                    }
                }

            }
            catch (Exception ee)
            {


                MessageBox.Show("조회 오류 : " + ee.Message);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }

        #endregion


        #endregion

        #region 유효성 검사

        private bool CheckData()
        {
            bool flag = true;

            return flag;
        }

        #endregion

        #region 데이터 그리드 체크박스 이벤트

        // 팝업창 체크박스 이벤트
        private void CHK_Click_Sub(object sender, RoutedEventArgs e)
        {
            //CheckBox chkSender = sender as CheckBox;
            //var MoveSub = chkSender.DataContext as Win_mtr_Move_U_CodeViewSub;

            //if (MoveSub != null)
            //{
            //    if (chkSender.IsChecked == true)
            //    {
            //        MoveSub.Chk = true;
            //        MoveSub.FontColor = true;

            //        if (ovcMoveSub.Contains(MoveSub) == false)
            //        {
            //            ovcMoveSub.Add(MoveSub);
            //        }
            //    }
            //    else
            //    {
            //        MoveSub.Chk = false;
            //        MoveSub.FontColor = false;

            //        if (ovcMoveSub.Contains(MoveSub) == true)
            //        {
            //            ovcMoveSub.Remove(MoveSub);
            //        }
            //    }
            //}
        }

        #endregion // 데이터 그리드 체크박스 이벤트

        #region 전체 선택 체크박스 이벤트

        // 전체 선택 체크박스 체크 이벤트
        private void AllCheck_Checked(object sender, RoutedEventArgs e)
        {
            //ovcMoveSub.Clear();

            //if (dgdMain.Visibility == Visibility.Visible)
            //{
            //    for (int i = 0; i < dgdMain.Items.Count; i++)
            //    {
            //        var MoveSub = dgdMain.Items[i] as Win_mtr_Move_U_CodeViewSub;
            //        MoveSub.Chk = true;
            //        MoveSub.FontColor = true;

            //        ovcMoveSub.Add(MoveSub);
            //    }
            //}
        }

        // 전체 선택 체크박스 언체크 이벤트
        private void AllCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            //ovcMoveSub.Clear();

            //if (dgdMain.Visibility == Visibility.Visible)
            //{
            //    for (int i = 0; i < dgdMain.Items.Count; i++)
            //    {
            //        var MoveSub = dgdMain.Items[i] as Win_mtr_Move_U_CodeViewSub;
            //        MoveSub.Chk = false;
            //        MoveSub.FontColor = false;
            //    }
            //}
        }

        #endregion // 전체 선택 체크박스 이벤트

        #region 기타 메서드

        // 천마리 콤마, 소수점 버리기
        private string stringFormatN0(object obj)
        {
            return string.Format("{0:N0}", obj);
        }


        // 데이터피커 포맷으로 변경
        private string DatePickerFormat(string str)
        {
            string result = "";

            if (str.Length == 8)
            {
                if (!str.Trim().Equals(""))
                {
                    result = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2);
                }
            }

            return result;
        }

        // Int로 변환
        private int ConvertInt(string str)
        {
            int result = 0;
            int chkInt = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Replace(",", "");

                if (Int32.TryParse(str, out chkInt) == true)
                {
                    result = Int32.Parse(str);
                }
            }

            return result;
        }

        // 소수로 변환 가능한지 체크 이벤트
        private bool CheckConvertDouble(string str)
        {
            bool flag = false;
            double chkDouble = 0;

            if (!str.Trim().Equals(""))
            {
                if (Double.TryParse(str, out chkDouble) == true)
                {
                    flag = true;
                }
            }

            return flag;
        }

        // 숫자로 변환 가능한지 체크 이벤트
        private bool CheckConvertInt(string str)
        {
            bool flag = false;
            int chkInt = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Trim().Replace(",", "");

                if (Int32.TryParse(str, out chkInt) == true)
                {
                    flag = true;
                }
            }

            return flag;
        }

        // 소수로 변환
        private double ConvertDouble(string str)
        {
            double result = 0;
            double chkDouble = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Replace(",", "");

                if (Double.TryParse(str, out chkDouble) == true)
                {
                    result = Double.Parse(str);
                }
            }

            return result;
        }






        #endregion // 기타 메서드

        // 메인 그리드 더블클릭시 선택한걸로!!
        private void dgdMain_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ClickCount == 2)
            //{
            //    btnConfirm_Click(null, null);
            //}
        }

        private void chkReq_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            var LotStock = chkSender.DataContext as Win_mtr_StockControl_U_CodeView;

            if (LotStock != null)
            {
                if (chkSender.IsChecked == true)
                {
                    LotStock.Chk = true;
                }
                else
                {
                    LotStock.Chk = false;
                }

            }
        }

        //제품그룹
        private void chkArticleGroup_Click(object sender, RoutedEventArgs e)
        {
            if (chkArticleGroup.IsChecked == true)
            {
                cboArticleGroup.IsEnabled = true;
                cboArticleGroup.Focus();
            }
            else
            {
                cboArticleGroup.IsEnabled = false;
            }
        }

        //제품그룹
        private void chkArticleGroup_Click(object sender, MouseButtonEventArgs e)
        {
            if (chkArticleGroup.IsChecked == true)
            {
                chkArticleGroup.IsChecked = false;
                cboArticleGroup.IsEnabled = false;

            }
            else
            {
                chkArticleGroup.IsChecked = true;
                cboArticleGroup.IsEnabled = true;
                cboArticleGroup.Focus();
            }
        }

        private void dtpAdjustDate_PreviewKeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dtpAdjustDate_CalendarClosed(object sender, RoutedEventArgs e)
        {

        }

       // 창고체크박스
        private void chkToLocSrh_Click(object sender, RoutedEventArgs e)
        {
            if (chkToLocSrh.IsChecked == true)
            {
                cboWareHouse.IsEnabled = true;
                cboWareHouse.Focus();
            }
            else
            {
                cboWareHouse.IsEnabled = false;
            }
        }

        // 창고
        private void chkToLocSrh_Click(object sender, MouseButtonEventArgs e)
        {
            if (chkToLocSrh.IsChecked == true)
            {
                chkToLocSrh.IsChecked = false;
                cboWareHouse.IsEnabled = false;

            }
            else
            {
                chkToLocSrh.IsChecked = true;
                cboWareHouse.IsEnabled = true;
                cboWareHouse.Focus();
            }
        }


        //품명 검색
        private void TxtArticleSrh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtArticleSrh, 77, "");
                //pf.ReturnCode(txtArticleSrh, 98, "");
            }
        }

        private void btnArticleSrh_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtArticleSrh, 77, "");
            //pf.ReturnCode(txtArticleSrh, 98, "");
        }
    }


}
