using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using WebAdmin.Models;

namespace WebAdmin.Controllers
{
    public class FileController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            if (null == files || 0 == files.Count)
            {
                return NotFound();
            }

            var file = files[0];
            var extension = Path.GetExtension(file.FileName);

            if (".xlsx" != extension)
            {
                return NotFound();
            }
 
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                var dtos = ReadExcelSheet(stream, true);

                foreach (var dto in dtos)
                {
                    Console.WriteLine($"{dto.Name}, {dto.Email}");
                }
            }

            ViewBag.Message = "File successfully uploaded.";

            return View(); // RedirectToAction("Index");
        }

        private IEnumerable<ContactDto> ReadExcelSheet(Stream stream, bool firstRowIsHeader)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
            {
                // Read the first Sheets 
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

                int counter = 0, index = 0;
                ContactDto dto;

                foreach (Row row in rows)
                {
                    counter = counter + 1;

                    // Read the first row as header
                    if (counter == 1)
                    {
                        // Ignore header
                    }
                    else
                    {
                        dto = new ContactDto();

                        index = 0;

                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            if (0 == index)
                            {
                                dto.Name = GetCellValue(doc, cell);
                            }
                            else if (1 == index)
                            {
                                dto.Email = GetCellValue(doc, cell);
                            }
                            else
                            {
                                throw new NotSupportedException("Column #{i} not supported.");
                            }

                            index++;
                        }

                        yield return dto;
                    }
                }
            }
        }

        private string GetCellValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = cell.CellValue.InnerText;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }

            return value;
        }
    }
}