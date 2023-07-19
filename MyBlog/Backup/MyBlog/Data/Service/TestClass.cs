using System.Net;
using System.Text;

namespace MyBlog.Data.Service
{
    public class TestClass
    {

        static byte[] ReadFileToBoundary(string filepath, string boundary)
        {
            // 시작 바운더리 설정
            var boundarybytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}\r\n");
            // 파일 읽어오기
            FileInfo fileInfo = new FileInfo(filepath);
            // 바운더리 해더 작성, Content-Type를 설정해도 됩니다. image/jpg 등, 특별한 설정이 없으면 application/octet-stream로 설정
            var header = Encoding.ASCII.GetBytes($"Content-Disposition: form-data; name=\"{fileInfo.Name}\"; filename=\"{fileInfo.Name}\"\r\nContent-Type: application/octet-stream\r\n\r\n");
            // 메모리 스트림으로 작성
            using (Stream stream = new MemoryStream())
            {
                // 바운더리 작성
                stream.Write(boundarybytes, 0, boundarybytes.Length);
                // 해더 작성
                stream.Write(header, 0, header.Length);
                // 파일 스트림 취득
                using (Stream fileStream = fileInfo.OpenRead())
                {
                    // 스트림 이동
                    fileStream.CopyTo(stream);
                }
                // stream을 byte로 변환할 버퍼 생성
                var ret = new byte[stream.Length];
                // 스트림 seek 이동
                stream.Seek(0, SeekOrigin.Begin);
                // stream을 byte로 이동
                stream.Read(ret, 0, ret.Length);
                // byte 반환
                return ret;
            }
        }
        static void Main(string[] args)
        {
            // 바운더리 설정
            string boundary = "**boundaryline**";
            // 접속할 url로 HttpWebRequest 설정
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://13.124.189.61:80/img");
            // ContentType 설정
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            // Method 설정
            request.Method = "POST";
            request.KeepAlive = true;
            // 파일을 읽어서 boundary 타입으로 변환하여 list로 설정
            // 파일 매소드로 변환
            var filelist = new List<byte[]>
      {
        ReadFileToBoundary(@"d:\\nowonbuntistory.png", boundary),
        ReadFileToBoundary(@"d:\\nowonbun.png", boundary),
      };
            // 종료 바운더리 설정
            var endboundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--");
            // 데이터 영역 크기 설정
            request.ContentLength = filelist.Sum(x => x.Length) + endboundary.Length;
            // 데이터 영역에 쓸 스트림 취득
            using (Stream stream = request.GetRequestStream())
            {
                // 파일 별 바운더리 설정
                foreach (var file in filelist)
                {
                    // 스트림에 바운더리 작성
                    stream.Write(file, 0, file.Length);
                }
                // 마지막 종료 바운더리 작성
                stream.Write(endboundary, 0, endboundary.Length);
            }
            // http 요청해서 결과를 받는다.
            using (var response = request.GetResponse())
            {
                // 결과 stream을 받아온다.
                using (Stream stream2 = response.GetResponseStream())
                {
                    // StreamReader로 변환
                    using (StreamReader reader2 = new StreamReader(stream2))
                    {
                        // 콘솔 출력
                        Console.WriteLine(reader2.ReadToEnd());
                    }
                }
            }
            // 아무 키나 누르면 종료
            Console.WriteLine("Press any key...");
            Console.ReadLine();
        }
    }
}
