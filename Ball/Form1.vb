Imports System.Data
Imports System.Drawing.Drawing2D
Imports System.ComponentModel

Public Class Form1
    Private ReadOnly Navy = Color.FromArgb(17, 43, 70), Blue = Color.FromArgb(29, 93, 145), Gold = Color.FromArgb(232, 185, 73), Danger = Color.FromArgb(201, 67, 67), Muted = Color.FromArgb(100, 119, 138)
    Private host, citizenPage, reportPage, policePage As Panel
    Private modeButton As Button
    Private reportTypeLabel As Label
    Private detailText, placeText, contactText, searchText As TextBox
    Private riskInput, priorityFilter, typeFilter As ComboBox
    Private anonymousCheck As CheckBox
    Private caseGrid As DataGridView
    Private cases As DataTable
    Private currentReportType = ""
    Private nextCaseNumber = 149

    Public Sub New()
        InitializeComponent()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint, True)
        BuildInterface() : SeedCases() : ShowPage(citizenPage)
    End Sub

    Private Sub BuildInterface()
        Dim header = New GradientPanel With {.Dock = DockStyle.Top, .Height = 94, .Color1 = Color.FromArgb(12, 35, 58), .Color2 = Color.FromArgb(24, 72, 108)}
        Dim mark = New RoundedPanel With {.BackColor = Gold, .Size = New Size(50, 50), .Location = New Point(28, 20), .CornerRadius = 15}
        mark.Controls.Add(New Label With {.Text = "✦", .Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleCenter, .ForeColor = Navy, .BackColor = Color.Transparent, .Font = New Font("Segoe UI Symbol", 21, FontStyle.Bold)})
        header.Controls.Add(mark)
        header.Controls.Add(New Label With {.Text = "สถานีรับแจ้งเหตุ 360°", .ForeColor = Color.White, .BackColor = Color.Transparent, .Font = New Font("Leelawadee UI", 20, FontStyle.Bold), .AutoSize = True, .Location = New Point(92, 17)})
        header.Controls.Add(New Label With {.Text = "บริการประชาชน  •  ปลอดภัย  •  ตรวจสอบได้", .ForeColor = Color.FromArgb(190, 209, 224), .BackColor = Color.Transparent, .AutoSize = True, .Location = New Point(94, 54)})
        modeButton = MakeButton("เข้าสู่ระบบตำรวจ  →", Color.White, Navy, 188, 46) : modeButton.Location = New Point(ClientSize.Width - 218, 23) : modeButton.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        AddHandler modeButton.Click, AddressOf ToggleMode : header.Controls.Add(modeButton) : modeButton.BringToFront()
        AddHandler header.Resize, Sub() modeButton.Location = New Point(Math.Max(0, header.ClientSize.Width - modeButton.Width - 32), 23)
        host = New Panel With {.Dock = DockStyle.Fill, .AutoScroll = True, .BackColor = BackColor}
        Controls.Add(host) : Controls.Add(New Panel With {.Dock = DockStyle.Top, .Height = 4, .BackColor = Gold}) : Controls.Add(header)
        citizenPage = BuildCitizenPage() : reportPage = BuildReportPage() : policePage = BuildPolicePage()
        host.Controls.AddRange({citizenPage, reportPage, policePage})
    End Sub

    Private Function BuildCitizenPage() As Panel
        Dim page = New Panel With {.Dock = DockStyle.Fill, .Padding = New Padding(48, 34, 48, 34), .AutoScroll = True}
        Dim layout = New TableLayoutPanel With {.Dock = DockStyle.Fill, .AutoSize = False, .ColumnCount = 2, .RowCount = 1}
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 62)) : layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 38))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        Dim left = New FlowLayoutPanel With {.Dock = DockStyle.None, .Anchor = AnchorStyles.None, .Size = New Size(940, 680), .FlowDirection = FlowDirection.TopDown, .WrapContents = False, .AutoSize = False, .AutoScroll = False, .Padding = New Padding(0, 6, 34, 0)}
        left.Controls.Add(New Label With {.Text = "●  จุดรับแจ้งเหตุด้วยตนเอง", .ForeColor = Blue, .Font = New Font(Font.FontFamily, 10, FontStyle.Bold), .AutoSize = True})
        left.Controls.Add(New Label With {.Text = "บอกเราได้ทุกเรื่อง" & vbCrLf & "เริ่มได้โดยไม่ต้องมีบัญชี", .ForeColor = Navy, .Font = New Font("Leelawadee UI", 34, FontStyle.Bold), .AutoSize = True, .Margin = New Padding(0, 10, 0, 10)})
        left.Controls.Add(New Label With {.Text = "เลือกประเภทเรื่องที่ใกล้เคียงที่สุด เจ้าหน้าที่จะตรวจสอบและส่งต่อให้หน่วยที่รับผิดชอบ", .ForeColor = Muted, .Font = New Font(Font.FontFamily, 12), .MaximumSize = New Size(720, 0), .AutoSize = True, .Margin = New Padding(0, 0, 0, 18)})
        Dim emergency = New RoundedPanel With {.Width = 900, .Height = 86, .BackColor = Color.FromArgb(255, 242, 240), .Margin = New Padding(0, 0, 0, 22), .CornerRadius = 18}
        emergency.Controls.Add(New Label With {.Text = "หากมีอันตรายเกิดขึ้นในขณะนี้" & vbCrLf & "ออกจากจุดเสี่ยงและโทรสายด่วนทันที", .ForeColor = Color.FromArgb(116, 67, 62), .Font = New Font(Font.FontFamily, 11, FontStyle.Bold), .AutoSize = True, .Location = New Point(18, 15)})
        Dim callButton = MakeButton("โทร 191", Danger, Color.White, 124, 48) : callButton.Location = New Point(752, 19)
        AddHandler callButton.Click, Sub() MessageBox.Show("กรุณาโทร 191 จากโทรศัพท์ของคุณ", "เหตุฉุกเฉิน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        emergency.Controls.Add(callButton) : left.Controls.Add(emergency)
        Dim categories = New TableLayoutPanel With {.ColumnCount = 2, .RowCount = 2, .Width = 900, .Height = 304}
        categories.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50)) : categories.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50)) : categories.RowStyles.Add(New RowStyle(SizeType.Percent, 50)) : categories.RowStyles.Add(New RowStyle(SizeType.Percent, 50))
        categories.Controls.Add(CategoryButton("อาชญากรรม / ทรัพย์สิน", "ลักทรัพย์ ฉ้อโกง ทำร้ายร่างกาย"), 0, 0) : categories.Controls.Add(CategoryButton("อุบัติเหตุ / จราจร", "รถชน กีดขวาง ถนนไม่ปลอดภัย"), 1, 0)
        categories.Controls.Add(CategoryButton("บุคคลสูญหาย", "เด็ก ผู้สูงอายุ หรือผู้ติดต่อไม่ได้"), 0, 1) : categories.Controls.Add(CategoryButton("เหตุรบกวน / เรื่องอื่น", "เสียงดัง ทะเลาะวิวาท เบาะแส"), 1, 1) : left.Controls.Add(categories)
        Dim info = New RoundedPanel With {.Dock = DockStyle.None, .Anchor = AnchorStyles.None, .Size = New Size(560, 650), .BackColor = Navy, .Padding = New Padding(42), .Margin = New Padding(14, 0, 0, 0), .CornerRadius = 28}
        Dim infoFlow = New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False}
        infoFlow.Controls.Add(New Label With {.Text = "ใช้เวลาประมาณ 5 นาที", .ForeColor = Gold, .Font = New Font(Font.FontFamily, 10, FontStyle.Bold), .AutoSize = True})
        infoFlow.Controls.Add(New Label With {.Text = "ข้อมูลที่ช่วยให้เราดำเนินการได้เร็วขึ้น", .ForeColor = Color.White, .Font = New Font(Font.FontFamily, 18, FontStyle.Bold), .MaximumSize = New Size(330, 0), .AutoSize = True, .Margin = New Padding(0, 12, 0, 18)})
        infoFlow.Controls.Add(StepPanel("1", "เกิดอะไรขึ้น", "เล่าตามที่เห็นหรือประสบด้วยตนเอง")) : infoFlow.Controls.Add(StepPanel("2", "เกิดที่ไหนและเมื่อไร", "ระบุจุดสังเกตหรือสถานที่ใกล้เคียง")) : infoFlow.Controls.Add(StepPanel("3", "ช่องทางติดต่อกลับ", "เลือกเปิดเผยหรือไม่ระบุตัวตน"))
        infoFlow.Controls.Add(New Label With {.Text = "ข้อมูลเปิดดูได้เฉพาะเจ้าหน้าที่ที่ได้รับสิทธิ์", .ForeColor = Color.FromArgb(185, 205, 220), .MaximumSize = New Size(320, 0), .AutoSize = True, .Margin = New Padding(0, 26, 0, 0)})
        info.Controls.Add(infoFlow) : layout.Controls.Add(left, 0, 0) : layout.Controls.Add(info, 1, 0) : page.Controls.Add(layout) : Return page
    End Function

    Private Function BuildReportPage() As Panel
        Dim page = New Panel With {.Dock = DockStyle.Fill, .Padding = New Padding(80, 38, 80, 44), .AutoScroll = True}, card = New RoundedPanel With {.Dock = DockStyle.Top, .Height = 610, .BackColor = Color.White, .Padding = New Padding(40), .CornerRadius = 26}
        Dim back = MakeButton("← กลับ", Color.White, Blue, 105, 40) : AddHandler back.Click, Sub() ShowPage(citizenPage)
        reportTypeLabel = New Label With {.Text = "ประเภทเหตุ", .ForeColor = Blue, .BackColor = Color.FromArgb(234, 243, 248), .AutoSize = True, .Padding = New Padding(10, 7, 10, 7), .Location = New Point(155, 35)}
        card.Controls.AddRange({back, reportTypeLabel, New Label With {.Text = "รายละเอียดการแจ้งเหตุ", .ForeColor = Navy, .Font = New Font("Leelawadee UI", 25, FontStyle.Bold), .AutoSize = True, .Location = New Point(34, 93)}})
        detailText = AddField(card, "เกิดอะไรขึ้น", 34, 150, 720, 88, True) : placeText = AddField(card, "สถานที่เกิดเหตุ", 34, 275, 350, 40)
        card.Controls.Add(New Label With {.Text = "วันและเวลาโดยประมาณ", .Location = New Point(414, 277), .AutoSize = True, .Font = New Font(Font, FontStyle.Bold)})
        card.Controls.Add(New DateTimePicker With {.Location = New Point(414, 304), .Width = 340, .Format = DateTimePickerFormat.Custom, .CustomFormat = "dd/MM/yyyy  HH:mm"})
        card.Controls.Add(New Label With {.Text = "สถานการณ์ตอนนี้", .Location = New Point(34, 374), .AutoSize = True, .Font = New Font(Font, FontStyle.Bold)})
        riskInput = New ComboBox With {.Location = New Point(34, 401), .Width = 350, .DropDownStyle = ComboBoxStyle.DropDownList} : riskInput.Items.AddRange({"เหตุการณ์จบแล้ว", "ยังเกิดต่อเนื่อง แต่ไม่มีอันตรายเร่งด่วน", "อาจมีผู้ได้รับอันตราย"}) : riskInput.SelectedIndex = 0 : card.Controls.Add(riskInput)
        contactText = AddField(card, "เบอร์ติดต่อกลับ (ไม่บังคับ)", 414, 374, 340, 40) : anonymousCheck = New CheckBox With {.Text = "ไม่ระบุตัวตน", .Location = New Point(34, 465), .AutoSize = True} : card.Controls.Add(anonymousCheck)
        Dim submit = MakeButton("ส่งเรื่องให้เจ้าหน้าที่", Blue, Color.White, 190, 46) : submit.Location = New Point(564, 520) : AddHandler submit.Click, AddressOf SubmitReport : card.Controls.Add(submit) : page.Controls.Add(card) : Return page
    End Function

    Private Function BuildPolicePage() As Panel
        Dim page = New Panel With {.Dock = DockStyle.Fill, .Padding = New Padding(40, 28, 40, 36)}
        page.Controls.Add(New Label With {.Text = "ศูนย์คัดกรองเหตุ  •  สน.กลางเมือง", .ForeColor = Blue, .Font = New Font(Font.FontFamily, 11, FontStyle.Bold), .AutoSize = True, .Location = New Point(35, 24)})
        page.Controls.Add(New Label With {.Text = "รายการรอตรวจสอบ", .ForeColor = Navy, .Font = New Font("Leelawadee UI", 26, FontStyle.Bold), .AutoSize = True, .Location = New Point(34, 51)})
        Dim stats = New FlowLayoutPanel With {.Location = New Point(34, 108), .Height = 90, .Width = 900, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = False}
        stats.Controls.Add(StatCard("รับเข้าวันนี้", "28", Blue)) : stats.Controls.Add(StatCard("เร่งด่วน", "4", Danger)) : stats.Controls.Add(StatCard("รอคัดกรอง", "11", Color.FromArgb(196, 132, 31))) : stats.Controls.Add(StatCard("ส่งต่อแล้ว", "17", Color.FromArgb(38, 114, 91)))
        Dim filters = New RoundedPanel With {.Location = New Point(34, 210), .Height = 88, .Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right, .BackColor = Color.White, .Width = 1100, .CornerRadius = 18}
        searchText = New TextBox With {.Location = New Point(20, 34), .Width = 290, .PlaceholderText = "ค้นหาเลขเคส สถานที่ หรือรายละเอียด"}
        priorityFilter = New ComboBox With {.Location = New Point(330, 34), .Width = 170, .DropDownStyle = ComboBoxStyle.DropDownList} : priorityFilter.Items.AddRange({"ทุกความเร่งด่วน", "เร่งด่วน", "ปกติ"}) : priorityFilter.SelectedIndex = 0
        typeFilter = New ComboBox With {.Location = New Point(520, 34), .Width = 220, .DropDownStyle = ComboBoxStyle.DropDownList} : typeFilter.Items.AddRange({"ทุกประเภท", "อาชญากรรม / ทรัพย์สิน", "อุบัติเหตุ / จราจร", "บุคคลสูญหาย", "เหตุรบกวน / เรื่องอื่น"}) : typeFilter.SelectedIndex = 0
        filters.Controls.AddRange({New Label With {.Text = "ตัวกรองรายการ", .Location = New Point(20, 10), .AutoSize = True, .ForeColor = Muted}, searchText, priorityFilter, typeFilter})
        caseGrid = New DataGridView With {.Location = New Point(34, 320), .Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right, .Width = 1100, .Height = 340, .BackgroundColor = Color.White, .BorderStyle = BorderStyle.None, .ReadOnly = True, .AllowUserToAddRows = False, .AllowUserToDeleteRows = False, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .RowHeadersVisible = False, .RowTemplate = New DataGridViewRow With {.Height = 52}}
        AddHandler page.Resize, Sub()
                                    filters.Width = Math.Max(800, page.ClientSize.Width - 68)
                                    caseGrid.Size = New Size(Math.Max(800, page.ClientSize.Width - 68), Math.Max(250, page.ClientSize.Height - 354))
                                End Sub
        AddHandler searchText.TextChanged, AddressOf ApplyFilters : AddHandler priorityFilter.SelectedIndexChanged, AddressOf ApplyFilters : AddHandler typeFilter.SelectedIndexChanged, AddressOf ApplyFilters
        Dim receive = MakeButton("รับเคสที่เลือก", Blue, Color.White, 160, 42) : receive.Anchor = AnchorStyles.Top Or AnchorStyles.Right : receive.Location = New Point(1020, 43)
        AddHandler page.Resize, Sub() receive.Left = page.ClientSize.Width - receive.Width - 35
        AddHandler receive.Click, AddressOf ReceiveCase : page.Controls.AddRange({receive, stats, filters, caseGrid}) : Return page
    End Function

    Private Sub SeedCases()
        cases = New DataTable() : For Each n In {"เลขเคส", "ความเร่งด่วน", "ประเภท", "รายละเอียด", "พื้นที่", "สถานะ"} : cases.Columns.Add(n) : Next
        cases.Rows.Add("PK-260923-0148", "เร่งด่วน", "อาชญากรรม / ทรัพย์สิน", "พบการทำร้ายร่างกายบริเวณหลังตลาด", "แขวงตลาด", "รอคัดกรอง") : cases.Rows.Add("PK-260923-0147", "เร่งด่วน", "บุคคลสูญหาย", "เด็กชายอายุ 8 ปีพลัดหลงจากผู้ปกครอง", "สวนสาธารณะริมคลอง", "รอคัดกรอง")
        cases.Rows.Add("PK-260923-0146", "ปกติ", "อุบัติเหตุ / จราจร", "รถเฉี่ยวชนแล้วหลบหนี มีภาพทะเบียน", "แยกถนนเหนือ", "รอคัดกรอง") : cases.Rows.Add("PK-260923-0145", "ปกติ", "เหตุรบกวน / เรื่องอื่น", "ร้านอาหารเปิดเสียงดังต่อเนื่อง", "แขวงตลาด", "รอคัดกรอง")
        caseGrid.DataSource = cases : caseGrid.EnableHeadersVisualStyles = False : caseGrid.ColumnHeadersDefaultCellStyle.BackColor = Navy : caseGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White : caseGrid.ColumnHeadersDefaultCellStyle.Font = New Font(Font, FontStyle.Bold) : caseGrid.ColumnHeadersHeight = 42 : caseGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(221, 237, 247) : caseGrid.DefaultCellStyle.SelectionForeColor = Navy : caseGrid.DefaultCellStyle.Padding = New Padding(5) : caseGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 251)
    End Sub

    Private Sub SubmitReport(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(detailText.Text) OrElse String.IsNullOrWhiteSpace(placeText.Text) Then MessageBox.Show("กรุณาระบุเหตุการณ์และสถานที่เกิดเหตุ", "ข้อมูลยังไม่ครบ", MessageBoxButtons.OK, MessageBoxIcon.Information) : Return
        Dim caseNo = $"PK-{Date.Today:yyMMdd}-{nextCaseNumber:0000}", priority = If(riskInput.Text.Contains("อันตราย"), "เร่งด่วน", "ปกติ") : nextCaseNumber += 1
        Dim row = cases.NewRow() : row.ItemArray = {caseNo, priority, currentReportType, detailText.Text.Trim(), placeText.Text.Trim(), "รอคัดกรอง"} : cases.Rows.InsertAt(row, 0)
        MessageBox.Show("เจ้าหน้าที่ได้รับเรื่องแล้ว" & vbCrLf & vbCrLf & "หมายเลขติดตาม: " & caseNo, "ส่งข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information) : detailText.Clear() : placeText.Clear() : contactText.Clear() : anonymousCheck.Checked = False : ShowPage(citizenPage)
    End Sub

    Private Sub ApplyFilters(sender As Object, e As EventArgs)
        If cases Is Nothing Then Return
        Dim q = searchText.Text.Replace("'", "''"), parts As New List(Of String)
        If q <> "" Then parts.Add($"([เลขเคส] LIKE '%{q}%' OR [รายละเอียด] LIKE '%{q}%' OR [พื้นที่] LIKE '%{q}%')")
        If priorityFilter.SelectedIndex > 0 Then parts.Add($"[ความเร่งด่วน] = '{priorityFilter.Text}'")
        If typeFilter.SelectedIndex > 0 Then parts.Add($"[ประเภท] = '{typeFilter.Text.Replace("'", "''")}'")
        cases.DefaultView.RowFilter = String.Join(" AND ", parts)
    End Sub

    Private Sub ReceiveCase(sender As Object, e As EventArgs)
        If caseGrid.CurrentRow Is Nothing Then Return
        Dim row = DirectCast(caseGrid.CurrentRow.DataBoundItem, DataRowView) : row("สถานะ") = "รับเคสแล้ว" : MessageBox.Show("รับเคส " & row("เลขเคส").ToString() & " เรียบร้อย", "มอบหมายงาน")
    End Sub

    Private Sub ToggleMode(sender As Object, e As EventArgs)
        If policePage.Visible Then modeButton.Text = "เข้าสู่ระบบตำรวจ  →" : ShowPage(citizenPage) Else modeButton.Text = "ออกจากระบบตำรวจ" : ShowPage(policePage)
    End Sub

    Private Sub ShowPage(page As Panel)
        citizenPage.Visible = False : reportPage.Visible = False : policePage.Visible = False : page.Visible = True : page.BringToFront()
    End Sub

    Private Function CategoryButton(title As String, description As String) As Button
        Dim b As New CategoryTile With {.TitleText = title, .DescriptionText = description, .BackColor = Color.White, .ForeColor = Navy, .AccentColor = Blue, .Dock = DockStyle.Fill, .Margin = New Padding(0, 0, 16, 16), .Tag = title, .Cursor = Cursors.Hand}
        AddHandler b.Click, Sub()
                                currentReportType = CStr(b.Tag)
                                reportTypeLabel.Text = currentReportType
                                ShowPage(reportPage)
                            End Sub
        Return b
    End Function

    Private Function StepPanel(number As String, title As String, detail As String) As Panel
        Dim p = New Panel With {.Width = 330, .Height = 82, .Margin = New Padding(0, 3, 0, 5)}
        p.Controls.Add(New Label With {.Text = number, .ForeColor = Gold, .BackColor = Color.FromArgb(37, 71, 99), .Font = New Font(Font.FontFamily, 12, FontStyle.Bold), .TextAlign = ContentAlignment.MiddleCenter, .Size = New Size(38, 38), .Location = New Point(0, 3)}) : p.Controls.Add(New Label With {.Text = title, .ForeColor = Color.White, .Font = New Font(Font, FontStyle.Bold), .AutoSize = True, .Location = New Point(52, 0)}) : p.Controls.Add(New Label With {.Text = detail, .ForeColor = Color.FromArgb(185, 205, 220), .MaximumSize = New Size(265, 0), .AutoSize = True, .Location = New Point(52, 25)}) : Return p
    End Function

    Private Function AddField(parent As Control, text As String, x As Integer, y As Integer, w As Integer, h As Integer, Optional multiline As Boolean = False) As TextBox
        parent.Controls.Add(New Label With {.Text = text, .Location = New Point(x, y), .AutoSize = True, .Font = New Font(Font, FontStyle.Bold), .ForeColor = Navy}) : Dim box = New TextBox With {.Location = New Point(x, y + 28), .Width = w, .Height = h, .Multiline = multiline, .BorderStyle = BorderStyle.FixedSingle, .BackColor = Color.FromArgb(249, 251, 252), .Font = New Font(Font.FontFamily, 11)} : parent.Controls.Add(box) : Return box
    End Function

    Private Function MakeButton(text As String, back As Color, fore As Color, w As Integer, h As Integer) As Button
        Dim b = New RoundedButton With {.Text = text, .BackColor = back, .ForeColor = fore, .FlatStyle = FlatStyle.Flat, .Width = w, .Height = h, .Font = New Font(Font.FontFamily, 10, FontStyle.Bold), .Cursor = Cursors.Hand, .CornerRadius = 13} : b.FlatAppearance.BorderSize = If(back = Color.White, 1, 0) : b.FlatAppearance.BorderColor = Color.FromArgb(210, 220, 228) : Return b
    End Function

    Private Function StatCard(label As String, value As String, accent As Color) As Panel
        Dim card = New RoundedPanel With {.Size = New Size(190, 78), .BackColor = Color.White, .CornerRadius = 16, .Margin = New Padding(0, 0, 12, 0)}
        card.Controls.Add(New Panel With {.BackColor = accent, .Dock = DockStyle.Left, .Width = 5})
        card.Controls.Add(New Label With {.Text = value, .ForeColor = accent, .Font = New Font("Leelawadee UI", 21, FontStyle.Bold), .AutoSize = True, .Location = New Point(22, 27)})
        card.Controls.Add(New Label With {.Text = label, .ForeColor = Muted, .Font = New Font(Font.FontFamily, 9), .AutoSize = True, .Location = New Point(22, 8)})
        Return card
    End Function
End Class

Public Class RoundedPanel
    Inherits Panel
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property CornerRadius As Integer = 18
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e) : ApplyRoundedRegion(Me, CornerRadius)
    End Sub
    Friend Shared Sub ApplyRoundedRegion(control As Control, radius As Integer)
        If control.Width < 2 OrElse control.Height < 2 Then Return
        Using path As New GraphicsPath()
            Dim d = radius * 2, r = New Rectangle(0, 0, control.Width - 1, control.Height - 1)
            path.AddArc(r.X, r.Y, d, d, 180, 90) : path.AddArc(r.Right - d, r.Y, d, d, 270, 90) : path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90) : path.AddArc(r.X, r.Bottom - d, d, d, 90, 90) : path.CloseFigure()
            control.Region = New Region(path)
        End Using
    End Sub
End Class

Public Class RoundedButton
    Inherits Button
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property CornerRadius As Integer = 12
    Private originalColor As Color
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl() : originalColor = BackColor
    End Sub
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e) : RoundedPanel.ApplyRoundedRegion(Me, CornerRadius)
    End Sub
    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        MyBase.OnMouseEnter(e) : BackColor = ControlPaint.Light(originalColor, 0.08F)
    End Sub
    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e) : BackColor = originalColor
    End Sub
End Class

Public Class CategoryTile
    Inherits Button
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property TitleText As String = ""
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property DescriptionText As String = ""
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property AccentColor As Color = Color.SteelBlue
    Private hovered As Boolean

    Public Sub New()
        FlatStyle = FlatStyle.Flat : FlatAppearance.BorderSize = 0 : Text = ""
        SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e) : RoundedPanel.ApplyRoundedRegion(Me, 18)
    End Sub

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        MyBase.OnMouseEnter(e) : hovered = True : Invalidate()
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e) : hovered = False : Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        e.Graphics.Clear(If(hovered, Color.FromArgb(244, 249, 252), BackColor))
        Using accentBrush As New SolidBrush(AccentColor)
            e.Graphics.FillEllipse(accentBrush, New Rectangle(22, 28, 48, 48))
        End Using
        Using iconFont As New Font("Segoe UI Symbol", 17, FontStyle.Bold), iconBrush As New SolidBrush(Color.White)
            Dim icon = If(TitleText.StartsWith("อาชญ"), "!", If(TitleText.StartsWith("อุบัติ"), "+", If(TitleText.StartsWith("บุคคล"), "?", "i")))
            Dim size = e.Graphics.MeasureString(icon, iconFont)
            e.Graphics.DrawString(icon, iconFont, iconBrush, 46 - size.Width / 2, 52 - size.Height / 2)
        End Using
        Using titleFont As New Font("Leelawadee UI", 12, FontStyle.Bold), detailFont As New Font("Leelawadee UI", 9.5F), titleBrush As New SolidBrush(ForeColor), detailBrush As New SolidBrush(Color.FromArgb(100, 119, 138))
            e.Graphics.DrawString(TitleText, titleFont, titleBrush, 88, 28)
            e.Graphics.DrawString(DescriptionText, detailFont, detailBrush, 88, 59)
            e.Graphics.DrawString("→", titleFont, titleBrush, Width - 42, 44)
        End Using
    End Sub
End Class

Public Class GradientPanel
    Inherits Panel
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Color1 As Color = Color.Navy
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Color2 As Color = Color.SteelBlue
    Public Sub New()
        SetStyle(ControlStyles.SupportsTransparentBackColor Or ControlStyles.OptimizedDoubleBuffer, True)
    End Sub
    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        Using brush As New LinearGradientBrush(ClientRectangle, Color1, Color2, LinearGradientMode.Horizontal)
            e.Graphics.FillRectangle(brush, ClientRectangle)
        End Using
    End Sub
End Class
