using QLSV.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLSV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB db = new StudentContextDB();

                // 1. Kiểm tra Mã SV đã tồn tại
                List<Student> studentList = db.Students.ToList();
                if (studentList.Any(s => s.StudentID == txtStudentID.Text))
                {
                    MessageBox.Show("Mã SV đã tồn tại. Vui lòng nhập một mã khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Khởi tạo đối tượng Student mới
                var newStudent = new Student
                {
                    StudentID = txtStudentID.Text,
                    FullName = txtFullname.Text,
                    // Giả định rD Male Checked là nam, nếu không là nữ. (Lưu ý: đoạn code trong hình bị thiếu dấu ngoặc nhọn kết thúc của rD Male Checked)
                    // Sửa lại để logic Gender rõ ràng hơn (dựa trên cấu trúc thường thấy)
                    Gender = rdMale.Checked ? "Male" : "Female", 

                    // Giả định rằng cmbFaculty.SelectedValue là một giá trị có thể chuyển thành int
                    FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString()),

                    // Giả định rằng txtAverageScore.Text là một giá trị có thể chuyển thành double
                    AverageScore = double.Parse(txtAverageScore.Text)
                };

                // 3. Thêm sinh viên vào CSDL
                db.Students.Add(newStudent);
                db.SaveChanges();

                // 4. Hiển thị lại danh sách sinh viên
                BindGrid(db.Students.ToList());

                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Hàm binding list có tên hiện thị là tên khoa, giá trị là Mã khoa
        private void FillFalcultyCombobox(List<Faculty> listFalcultys)
        {
            this.cmbFaculty.DataSource = listFalcultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }
        //Hàm binding gridView từ list sinh viên
        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                dgvStudent.Rows[index].Cells[3].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[2].Value = item.AverageScore;
                dgvStudent.Rows[index].Cells[4].Value = item.Gender;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB db = new StudentContextDB();

                // Lấy danh sách sinh viên hiện tại và tìm sinh viên cần cập nhật theo StudentID trong TextBox
                List<Student> studentList = db.Students.ToList();
                var student = db.Students.FirstOrDefault(s => s.StudentID == txtStudentID.Text);

                // Kiểm tra xem sinh viên có được tìm thấy không
                if (student != null)
                {
                    // Logic kiểm tra trùng lặp StudentID khi cập nhật:
                    // Nếu có bất kỳ sinh viên nào KHÔNG phải là sinh viên đang cập nhật (s.StudentID != student.StudentID) 
                    // mà lại có StudentID trùng với giá trị trong TextBox (txtStudentID.Text), thì báo lỗi.
                    // **LƯU Ý:** Đoạn code trong hình có vẻ thừa điều kiện `s.StudentID == txtStudentID.Text`
                    // vì nó đã được kiểm tra ở dòng trên. Tuy nhiên, tôi chép lại chính xác code trong hình:
                    if (studentList.Any(s => s.StudentID != student.StudentID && s.StudentID == txtStudentID.Text))
                    {
                        MessageBox.Show("Mã SV đã tồn tại. Vui lòng nhập một mã khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Cập nhật thông tin sinh viên với dữ liệu mới từ các Controls
                    student.FullName = txtFullname.Text;
                    student.Gender = rdMale.Checked ? "Male" : "Female";
                    student.FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString());
                    student.AverageScore = double.Parse(txtAverageScore.Text);

                    // Cập nhật sinh viên lưu vào CSDL
                    db.SaveChanges();

                    // Hiển thị lại danh sách sinh viên
                    BindGrid(db.Students.ToList());

                    MessageBox.Show("Chỉnh sửa thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sinh viên không tìm thấy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB db = new StudentContextDB();
                List<Student> studentList = db.Students.ToList();

                // Tìm kiếm sinh viên có tồn tại trong CSDL hay không
                var student = studentList.FirstOrDefault(s => s.StudentID == txtStudentID.Text);

                if (student != null)
                {
                    // Xoá sinh viên khỏi CSDL
                    db.Students.Remove(student);
                    db.SaveChanges();

                    BindGrid(db.Students.ToList());

                    MessageBox.Show("Sinh viên đã được xoá thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sinh viên không tìm thấy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                StudentContextDB context = new StudentContextDB();
                List<Faculty> listFalcultys = context.Faculties.ToList(); //lấy các khoa
                List<Student> listStudent = context.Students.ToList(); //lấy sinh viên
                FillFalcultyCombobox(listFalcultys);
                BindGrid(listStudent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvStudent.Rows[e.RowIndex];
                txtStudentID.Text = selectedRow.Cells[0].Value.ToString();
                txtFullname.Text = selectedRow.Cells[1].Value.ToString();
                string gender = selectedRow.Cells[4].Value.ToString();

                if (gender == "Male")
                {
                    rdMale.Checked = true;
                }
                else
                {
                    rdFemale.Checked = true;
                }

                txtAverageScore.Text = selectedRow.Cells[2].Value.ToString();
                cmbFaculty.Text = selectedRow.Cells[3].Value.ToString();
            }
        }
    }


}
