// 2024-04-10 Checked by RZN
// Scriptable Object 를 사용하기로 했기 때문에, 필요하지 않을 가능성이 있습니다.

using System.Data;
using System.IO;
using ExcelDataReader;

namespace AlienProject
{
	public class DBManager
	{
		public static DataTable ConvertExcelToDataTable(string filePath)
		{
			using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
			{
				using (var reader = ExcelReaderFactory.CreateReader(stream))
				{
					// 첫 번째 시트 선택
					var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
					{
						ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
					});

					// 첫 번째 테이블 선택
					var dataTable = dataSet.Tables[0];

					return dataTable;
				}
			}
		}
	}
} // namespace AlienProject